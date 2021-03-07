using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rhinos
{
    public class PhoneBook
    {
        string filename;
        public class Entry
        {
            public string Name;
            public string Phone;
            public string Type;

            public Entry(string Name, string Phone, string Type)
            {
                this.Name = Name;
                this.Phone = Phone;
                this.Type = Type;
            }
        }

        public PhoneBook(string filename)
        {
            this.filename = filename;
        }

        /*
         * Args:
         *      1. name (string)
         * 
         * Note:
         *  Since we always adding new Items at the end, the FIRST record from the end 
         *  is the most updated.
         *      
         * Time complexity:  O(n)
         * 
         */
        public Entry GetByName(string name, bool fromMain=false)
        {
            Entry _contact = new Entry("", "", "");

            // In case method is called from main, we reset phonebook text to be ready for returning list. (1 call)
            // When this Method is called from Building sorted response, we DONT reset phonebook text to be ready for returning list. (to many calls)
            if (fromMain)
            {
                string first_line = GetFirstLine();

                if (first_line.Contains("0"))
                {
                    // If we recognize that call from Method is from main, and List is not set (meaning, leaing bit is '0' on file)
                    // We prepare to file for get all users command.
                    ReturnSlowList();
                }
 
            }
            List<string> lines = File.ReadAllLines(this.filename).ToList();
            string[] parts;

            for (int i = lines.Count-1; i>0; i--)
            {
                parts = lines[i].Split(",");
                if (parts[0].Equals(name))
                {
                    _contact = new Entry(parts[0], parts[1], parts[2]);
                    break;
                }

            }
            return _contact;
   
        }

        /*
         * Each time we Insert a record, we push it to the end of the file
         * Meaning, for each unique_name in our lines, the most updated one is the most relvant for us.
         * Pushing to the end of the file is dont by AppendText, very quickly.
         */
        public void InsertOrUpdate(Entry e)
        {
            Change_first_line("0");
            AddText(GetEntryForInsertion(e));

        }


        /*
        * Note:
        *      Each time we Wish to return List of our Entries, we check leading bit on the file. 
        *      case 0 -> 
        *           File has been manipulated, need to prepare file
        *           Procces is ->
        *                       1. fetching all unique_name in the text file -> O(N'), 
        *                           N' := is givven by sum of duplicated rows.
        *                           N  := actual unieuq_users in PhoneBook
        *                               Important -> At each time we activate Method ReturnSlowList() We Align N' to be equal to N
        *                       2. constructing Array of entries by SORTED array of N unique_name -> O(N*log(N))
        *        
        *       case 1 -> 
        *           Procces of ReturnSlowList() had just made,
        *           2 possible ways of invoking ReturnSlowList() Method.
        *               1. Returning Iterator of all clients, and leading bit is 0. (Meaning we just pushed at least 1 contact)
        *               2. Returning single Contact by Name a a direct order from main.
        *               3. O(N), where N is number of unique_users. (its already sorted).
        *               
        */
        public IEnumerable<Entry> Iterate()
        {

            string first_line = GetFirstLine();

            if (first_line.Contains("1"))
            {
                // If List is already sorted and ready, we return it.
                return ReturnFastList();
            }
            else
            {
                return ReturnSlowList();

            }

        }

        /*
         * **********************************************************
         * **********************************************************
         * ************************ Helpers *************************
         * **********************************************************
         * **********************************************************
         */


        /*
 * Returning List, as File is already ready,
 * Meaning all its records are Sorted.
 */
        private List<Entry> ReturnFastList()
        {
            // Lines are already sorted and ready.
            string[] readText = File.ReadAllLines(this.filename);
            List<Entry> entries_Order = new List<Entry> { };
            Entry temp;
            for (int i = 1; i < readText.Length; i++)
            {
                // again, starting form index 1
                // since first line is always a '0' or '1'
                // Indicating status of file readiness for returning all contacts as response. 
                string[] parts = readText[i].Split(",");
                temp = new Entry(parts[0], parts[1], parts[2]);
                entries_Order.Add(temp);
            }
            return entries_Order;
        }
        private List<Entry> ReturnSlowList()
        {

            string[] readText = File.ReadAllLines(this.filename);
            string[] names = GetSortedNames().ToArray();
            List<Entry> entries_Order = new List<Entry> { };
            foreach (string t in names)
            {

                entries_Order.Add(GetByName(t));
            }


            ReWriteText(entries_Order);

            return entries_Order;
        }

        private string GetEntryForInsertion(Entry e)
        {
            // Helper method, for easy appending file records.
            return e.Name + "," + e.Phone + "," + e.Type;

        }

        public void DisplayContact(Entry e)
        {
            // Helper method, for easy Consoling signle file record.
            Console.WriteLine(e.Name.PadRight(30) + " " + e.Phone.PadRight(25) + " " + e.Type.PadRight(25));
        }

        public bool CheckIfFileExist(string pathToFile)
        { 
           return File.Exists(pathToFile);
        }

        public void CreateFile(string pathToFile)
        {
            try
            {
                using (FileStream fs = File.Create(this.filename))
                {
                }
                // Creating the file, and adding 0 Meaning file is not ready yet to be sent.
                AddText("0");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /*
         * a single method to control the leading bit we have at the begining of the file.
         * c (char): could be '0' or '1' only.
         */
        private void Change_first_line(string c)
        {
            byte[] data = Encoding.ASCII.GetBytes(c);
            using (Stream stream = File.Open(filename, FileMode.Open))
            {
                stream.Position = 0;
                stream.Write(data, 0, data.Length);
            }
        }
        
        private void ReWriteText(List<Entry> entries_Order)
        {;
            File.Delete(this.filename);
            using (FileStream fs = File.Create(this.filename))
            {
            }
            AddText("1"); // 1 symbols its fixed, for fast response.
            foreach (Entry e in entries_Order)
            {
                AddText(GetEntryForInsertion(e));
            }
        }
        
        private string[] GetSortedNames()
        {
            List<string> lines = File.ReadAllLines(this.filename).ToList();
            string[] sortedNames_Helper = new string[lines.Count-1];
            string[] parts;
            int count = lines.Count-1;
            string tempName = "";
            int idx = 0;
            
            for(int i= count; i>0; i--)
            {
                parts = lines[i].Split(",");
                tempName = parts[0];
                    
                if (sortedNames_Helper.Contains(tempName)) continue;
                else
                {
                    sortedNames_Helper[idx] = tempName;
                    idx += 1;
                }
            }
            //Array.Sort(sortedNames);
            string[] sortedNames = new string[idx];

            for (int j = 0; j< idx; j++)
            {
                sortedNames[j] = sortedNames_Helper[j];
            }

            Array.Sort(sortedNames);
            return sortedNames;
        }
       
        private string GetFirstLine()
        {
            string firstLine = "";

            using (StreamReader reader = new StreamReader(this.filename))
            {
                firstLine = reader.ReadLine();
          
            }
            return firstLine;

        }

        private void AddText(string line)
        {
            using (StreamWriter sw = File.AppendText(this.filename))
            {
                sw.WriteLine(line);
            }
        }

    }
}
