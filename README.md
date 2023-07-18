# BSharp
BSharp is my own programming language I made. It probably is not optimized but atleast it works.

 
## Formatting:

Running a file - *BSharp [file location] (BSharp mycodefile.bs)*

Determeting version - *BSharp ver(sion)*
 

## Code formatting:

exit - Exits the enviroment

readline [variable] [text] - Reads input and puts it in an already declared variable

wait [ms] - Waits the specified time in miliseconds

hide - Hides the console window.

show - Shows the console window.

print [text] - Prints text on the screen (Use | for a space)

pause - Pauses and waits for next key input

add [1] [2] - Adds two numbers up and prints the output

clear - Clears the screen.

var [name] [value] - Makes a new variable for later use.

get [type] [variable] - Gets a specified value and puts the output into an declared variable


### Get types
username - Computer username
machinename - Machine name
userdomainname - User domain name
osversion - OS version
systemdirectory - System directory


## Examples

Asking the user for his name and returning it back
```
var yourname somename

print What|is|your|name?
readline yourname

print Hello,
print $yourname

pause
exit
```

Getting system information
```
var os null
var user null
var domain null

get osversion os
get username user
get userdomainname domain

print $os
print $user
print $domain

pause
exit
```
