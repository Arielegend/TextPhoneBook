using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rhinos
{
    class Program
    {
        static void Main(string[] args)
        {
            // Inital messages, explaining about the program. 
            // Only shows at opening program
            string _pathToFile = System.IO.Directory.GetCurrentDirectory() + "\\PhoneBook.txt";
            Console.WriteLine("Path to PhoneBook file ->\n" + _pathToFile.ToString()+"\n");
            PhoneBook _phoneBook = new PhoneBook(_pathToFile);
            List<PhoneBook.Entry> _phoneBookIterator;

            if (!_phoneBook.CheckIfFileExist(_pathToFile))
            {
                _phoneBook.CreateFile(_pathToFile);
            }

         
            ShowInitialMessage();

            bool _running = true;
            while (_running)
            {
                // Getting user input for wanted action.
                string _userInput = ShowMessagesAndGetUserInput();
                Console.Clear();

                switch (_userInput)
                {
                    case "1":
                        AddContact(_phoneBook);
                        Console.Clear();
                        break;

                    case "2":
                        DisplayContact(_phoneBook);
                        Console.Clear();
                        break;

                    case "3":
                        _phoneBookIterator = (List<PhoneBook.Entry>)_phoneBook.Iterate();
                        Console.WriteLine("\nSuuccessfully got PhonBook list");
                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case "4":
                        _phoneBookIterator = (List<PhoneBook.Entry>)_phoneBook.Iterate();
                        foreach (PhoneBook.Entry e in _phoneBookIterator)
                        {
                            _phoneBook.DisplayContact(e);
                        }
                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case "5":
                        Console.WriteLine("Shutting down...");
                        _running = false;
                        break;


                    default:
                        Console.WriteLine("Please enter a valid digit in range [1-5]");
                        break;
                }

            }
        }

        /* Args:
         *      1. inputType (int) - represents the input type we wish to check.
         *                       1 := Checking Name  input (Names must be only letters, no spaces, including '_')
         *                       2 := Checking Phone input (Phones must be only digits)
         *                       3 := Checking Type  input (Type must be only letters)
         *      
         *      2. inputToCheck (string) - the input-string we wish to check.
         *        
         * Returns:
         *      Boolean indicates weather input is valid or not.
         */
        private static bool CheckInput(int inputType, string inputToCheck)
        {
            switch (inputType)
            {
                //Checking Name input
                //Checking Only letters and '_' symbol
                case 1:
                    return Regex.IsMatch(inputToCheck, @"^[a-zA-Z_]+$");

                //Checking Phone input
                //Checking Only digits
                case 2:
                    return Regex.IsMatch(inputToCheck, @"^[0-9]+$");

                //Checking Type input
                //Checking Only letters
                case 3:
                    return Regex.IsMatch(inputToCheck, @"^[a-zA-Z]+$");

                default:
                    break;
            }
            return true;
        }

        /*
         * Initial Method.
         * Displays PhoneBook menu options. 
         * 
         * Returns a string indicates the user action.
         */
        private static string ShowMessagesAndGetUserInput()
        {
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("\t (1) - Insert / Update new entry");
            Console.WriteLine("\t (2) - Get by name");
            Console.WriteLine("\t (3) - Get phone-book");
            Console.WriteLine("\t (4) - Display phone-book");
            Console.WriteLine("\t (5) - Exit");
            string _userInput = Console.ReadLine();
            return _userInput;
        }

        /*
         * At this method we gather information from user about requested new contact.
         * There are 3 main while loops, each loop will break once the user puts valid inputs.
         *      1. Name  ->  May be of form <privateName>_<Surname>
         *      2. Phone ->  May contain only digits
         *      3. Type  ->  May contain only latters. 
         */
        private static void AddContact(PhoneBook _phoneBook)
        {
            string _nameToAdd = "";
            string _numberToAdd = "";
            string _typeOfNumber = "";

            // 3 Booleans indicating users inputs for adding / updating contact.
            bool _okName = false;
            bool _okPhone = false;
            bool _okType = false;

            while (!_okName)
            {
                Console.WriteLine("Name to add:");
                _nameToAdd = Console.ReadLine();
                _nameToAdd = _nameToAdd.TrimEnd();
                _okName = CheckInput(1, _nameToAdd);
            }

            while (!_okPhone)
            {
                Console.WriteLine("\n" + _nameToAdd + " number is:");
                _numberToAdd = Console.ReadLine();
                _numberToAdd = _numberToAdd.TrimEnd();
                _okPhone = CheckInput(2, _numberToAdd);
            }

            while (!_okType)
            {
                Console.WriteLine("\nType of number is:");
                _typeOfNumber = Console.ReadLine();
                _typeOfNumber = _typeOfNumber.TrimEnd();
                _okType = CheckInput(3, _typeOfNumber);
            }

            PhoneBook.Entry _entry = new PhoneBook.Entry(_nameToAdd, _numberToAdd, _typeOfNumber);
            _phoneBook.InsertOrUpdate(_entry);
            Console.WriteLine("\nSuccessfuly added new contact\n" + _entry.Name + " " + _entry.Phone + " " + _entry.Type);
            Console.ReadLine();
        }

        private static void DisplayContact(PhoneBook _phoneBook)
        {
            Console.WriteLine("Enter contact name:");
            string _contactName = Console.ReadLine();

            //Removing spaces from right side.
            _contactName = _contactName.TrimEnd();
            bool _fromMain = true;
            PhoneBook.Entry _contact = _phoneBook.GetByName(_contactName, _fromMain);

            // GetByName always returns an Entry. 
            // In case no match, Entry will be empty.
            if (_contact.Name.Length > 0)
            {
                Console.WriteLine("");
                _phoneBook.DisplayContact(_contact);

            }
            else
            {
                // If entered else -> We recived an empty Entry, meaning no match is found.
                Console.WriteLine("\nOops.. seems there is no matching contact!");
            }

            Console.ReadLine();
        }

        private static void ShowInitialMessage()
        {
            Console.WriteLine("Welcome to Rhinos PhoneBook");
            Console.WriteLine("PhoneBook file is located at working directory");
            Console.WriteLine("To add new contact ->");
            Console.WriteLine("\t<name_surname> (i.e  Baby_Rhino, Madona, Madona_the_second");
            Console.WriteLine("\t<phone> (Only digits)");
            Console.WriteLine("\t<type> (Only letters)");
            Console.WriteLine("\nHit Enter when ready...");
            Console.ReadLine();
            Console.Clear();
        }

         
    }

}