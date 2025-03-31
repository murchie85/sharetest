```
// Function to remove non-printable characters
string RemoveNonPrintableChars(string input)
{
    return new string(input.Where(c => !char.IsControl(c)).ToArray());
}

// Apply the function to your crt string
string crtCleaned = RemoveNonPrintableChars(crt);
```