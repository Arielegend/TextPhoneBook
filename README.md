# TextPhoneBook

### Flow

<p>
On creation, at working directory a new text file name 'PhoneBook.txt' will be created,
with a leading '0' at the first line.
</p>

<p>
Each time we add a new contact, we do so by AppendText, a quick performance tool.
Therefore, for 2 records with sasme user_name field, the lower one, is more updated. FOR each unique_name.
</p>

<p>
At each time we call for a certain Entry, from main command, we also ordering the file for faster response,
Assining '1' as leading bit of the file
</p>

<p>
As well as each time we call Iterate, 
If file is already 'SET to go' (meaning all recordfs are already sorted, and unique by name)
we wiil return FAST_LIST.

Otherwise,
we prepare the file,
and set the leading bit to be 1
</p>
