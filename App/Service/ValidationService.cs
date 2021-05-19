using System.Net.Mail;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System;
namespace Service
{
    public static class ValidationService
    {
        /// <summary>
        /// Returns False When Regex does not match
        /// </summary>
        /// <param name="input">String to be validated</param>
        public static bool ValidatePersonName(string input)
        {
            string pattern = @"^[A-Z][A-Za-z]+ [A-Z][A-Za-z]+$";
            return ValidateFromRegex(input, pattern);
        }
        /// <summary>
        /// Returns False When Regex does not match
        /// </summary>
        /// <param name="input">String to be validated</param>
        public static bool ValidateCityName(string input)
        {
            string pattern = @"^[A-Za-z \.']+$";
            return ValidateFromRegex(input, pattern);
        }
        /// <summary>
        /// Returns False When Regex does not match
        /// </summary>
        /// <param name="input">String to be validated</param>
        public static bool ValidateString(string input)
        {
            return !String.IsNullOrWhiteSpace(input);
        }
        /// <summary>
        /// Returns False When Regex does not match
        /// </summary>
        /// <param name="input">String to be validated</param>
        public static bool ValidateDouble(string input)
        {
            string pattern = @"^(-?)(0|([1-9][0-9]*))(\.[0-9]+)?$";
            return ValidateFromRegex(input, pattern);
        }
        /// <summary>
        /// Returns False When Regex does not match
        /// </summary>
        /// <param name="input">String to be validated</param>
        public static bool ValidateAddress(string input)
        {
            string pattern= @"^[#.0-9a-zA-Z\s,-]+$";
            if(ValidateString(input)){
                return ValidateFromRegex(input, pattern);
            }else{
                return false;
            }
            
        }
        /// <summary>
        /// Returns False When Email cannot be Converted to MailAddress
        /// </summary>
        /// <param name="input">String to be validated</param>
        public static bool ValidateEmail(string input)
        {   
            try{
                MailAddress addr = new MailAddress(input);
            }catch(Exception){
                return false;
            }
            return true;
        }
        /// <summary>
        /// Returns False When Regex does not match
        /// </summary>
        /// <param name="input">String to be validated</param>
        public static bool ValidateInt(string input)
        {
            string pattern = @"^[+-]?[0-9]+$";
            return ValidateFromRegex(input, pattern);
        }
        /// <summary>
        /// Returns False When Regex does not match(Leading + is optional)
        /// </summary>
        /// <param name="input">String to be validated</param>
        public static bool ValidatePositiveInt(string input)
        {
            string pattern = @"^[+]?[0-9]+$";
            return ValidateFromRegex(input, pattern);
        }
        /// <summary>
        /// Returns False When Regex does not match(Must have leading -)
        /// </summary>
        /// <param name="input">String to be validated</param>
        public static bool ValidateNegativeInt(string input)
        {
            string pattern = @"^[-][0-9]+$";
            return ValidateFromRegex(input, pattern);
        }
        ///<summary>
        /// Returns False When Regex does not match && Input is not within low,high
        /// Inclusive
        /// </summary>
        /// <param name="input">String to be validated</param>
        /// <param name="low">Lowest Number Accepted</param>
        /// <param name="high">Highest Number Accepted</param>        
        public static bool ValidateIntWithinRange(string input, int low, int high)
        {
            string pattern= @"^[+-]?[0-9]+$";
            if(ValidateFromRegex(input, pattern))
            {
                return ValidateWithinRange(int.Parse(input), low, high);
            }else{
                return false;
            }
        }
        private static bool ValidateFromRegex(string input, string pattern)
        {
            Regex rx = new Regex(pattern);
            return rx.IsMatch(input);
        }
        private static bool ValidateWithinRange(int input, int low, int high)
        {
            return ((input <= high)&&(input >= low));
        }
        
        
    }
}