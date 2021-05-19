using System.Collections.Generic;
using System;
using Service;
using Serilog;
namespace UI
{
    public static class SelectFromList
    {
        public static Object Start(List<Object> objects)
        {
            string input;
            do{
                Console.Clear();
                Console.WriteLine("Choose From List:");
                Console.WriteLine("[0] Cancel Selection");
                int i = 1;
                foreach (object item in objects)
                {
                    Console.WriteLine("[{0}] {1}", i++, item.ToString());
                }
                Console.WriteLine("Enter Your Selection:");
                input = Console.ReadLine();
                if(input == "0"){
                    Log.Information("0 selected In Selection Menu");
                    throw new NullReferenceException("Selection Canceled");
                  
                }
            }while(!ValidationService.ValidateIntWithinRange(input, 1, objects.Count));

           int selection = int.Parse(input);
           Object retVal =  objects[selection-1];
           return retVal;
            
        }
    }
}