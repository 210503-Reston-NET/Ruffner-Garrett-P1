using System;

namespace UI
{
    public interface IValidationUI
    {
        // string ValidateString(string prompt);

        // int ValitdateInt(string prompt);

        // bool ValidateCityName(string input);

        // bool ValidatePersonName(string input);

        string ValidationPrompt(string prompt, Func<string, bool> validate);
    }
}