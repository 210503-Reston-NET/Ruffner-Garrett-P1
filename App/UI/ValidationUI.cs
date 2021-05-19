using System;
namespace UI
{
    public class ValidationUI: IValidationUI
    {
        public string ValidationPrompt(string prompt, Func<string, bool> validate){
            string response;
            bool valid = false;
            do{
                Console.Clear();
                if(valid){
                    Console.WriteLine("Invalid Entry");
                }
               Console.WriteLine(prompt);
               response = Console.ReadLine();
               valid = true;
            }while(!validate(response));
            return response;

        }
    }
}
