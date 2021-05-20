using Service;
using Serilog;
using System;
namespace UI
{
    public class OrderMenu : IMenu
    {
        private IService _services;
        private IValidationUI _validate;


        public OrderMenu(IService services, IValidationUI validate)
        {
            _services = services;
            _validate = validate;
        }

        public void Start()
        {
            Log.Verbose("Started Order Menu");
            bool repeat = true;
            do{
                Console.Clear();
                Console.WriteLine("Order Menu:");
                // Console.WriteLine("Selected Customer: {}", _customer.Name);
                Console.WriteLine("[0] List Orders Newest First");
                Console.WriteLine("[1] List Orders Oldest First");
                Console.WriteLine("[2] Create New Order");
                Console.WriteLine("[3] Exit");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "0":
                        //List Orders Newest First

                    break;
                    case "1":
                        //List orders Oldest First

                    break;
                    case "2":
                        //Create new Order

                    break;
                    case "3":
                        Log.Information("Closing Order Menu");
                        repeat = false;
                    break;
                    default:
                    Console.WriteLine("Choose valid option");
                    break;

                }
            } while(repeat);
        }
    }
}