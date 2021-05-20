using System;
using Service;
using Serilog;
using StoreModels;
using System.Collections.Generic;
using System.Linq;

namespace UI
{
    public class InventoryMenu : IMenu
    {
        private IService _services;
        private IValidationUI _validate;

        private Location _location;
        public InventoryMenu(IService services, IValidationUI validate)
        {
            _services = services;
            _validate = validate;
        }
        public void Start()
        {
            //First Get A Location
            try{ 
                List<Object> objectList = _services.GetAllLocations().Cast<Object>().ToList<Object>();
                
                Object ret = SelectFromList.Start(objectList);
                _location = (Location) ret;
                

            }catch(NullReferenceException ex){
                Log.Verbose("Returned null from Location Selection", ex, ex.Message);
                Console.WriteLine("Cancelled Location Selection");
                Console.WriteLine("Press Any Key to Continue ...");
                Console.ReadKey();
                return;
            }catch(Exception ex){
                Log.Error(ex, ex.Message);
                return;
            }

            //Do actions with inventory
            bool repeat = true;
            string str;
            do{
                Console.Clear();
                Console.WriteLine("Inventory Menu For Location:\n{0}",_location.ToString());
                Console.WriteLine("[0] Exit");
                Console.WriteLine("[1] View Inventory Of Location");
                Console.WriteLine("[2] Update Inventory");
                Console.WriteLine("[3] Add Product To Inventory");

                
                string input = Console.ReadLine();
                switch (input)
                {
                    case "0":
                        //Exit Menu
                        Console.WriteLine("Bye");
                        Log.Information("program exit from menu");
                        repeat = false;
                    break;
                    case "1":
                        // View Inventory Of Location
                        try{
                            Console.WriteLine("{0} Inventory:", _location.ToString());
                            foreach (Item item in _location.InventoryItems)
                            {
                                Console.WriteLine(item.ToString());
                            }
                            Console.WriteLine();
                            Console.WriteLine("Press Any Key to Continue ...");
                            Console.ReadKey();
                        }catch(Exception ex){
                            Log.Error("Error Viewing Location", ex.Message);
                        }
                    break;
                    case "2":
                        //Update Inventory
                        //Get Product from inventory
                        Item selectedItem;
                        try{ 
                            List<Object> objectList = _location.InventoryItems.Cast<Object>().ToList<Object>();
                            
                            Object ret = SelectFromList.Start(objectList);
                            selectedItem = (Item) ret;
                            
                        }catch(NullReferenceException ex){
                            Log.Verbose("Returned null from Item Selection", ex, ex.Message);
                            Console.WriteLine("Cancelled Item Selection");
                            Console.WriteLine("Press Any Key to Continue ...");
                            Console.ReadKey();
                            break;
                        }catch(Exception ex){
                            Log.Error(ex, ex.Message);
                            break;
                        }
                        //increase or decrease stock
                        str = _validate.ValidationPrompt("Enter Amount increase/decrease stock by:", ValidationService.ValidateInt);
                        
                        try{
                            //try updating item quantity
                            _services.updateItemInStock(_location, selectedItem, int.Parse(str));
                            Console.WriteLine("Stock updated");
                        }catch(Exception ex){
                            Console.WriteLine(ex.Message);
                            Console.WriteLine("Stock Could Not Be Updated");
                        }
                        Console.WriteLine("Press Any Key to Continue ...");
                        Console.ReadKey();

                    break;
                    case "3":
                        Product prod;
                        //Add New Product to Inventory
                        try{ 
                            List<Object> objectList = _services.GetAllProducts().Cast<Object>().ToList<Object>();
                            
                            Object ret = SelectFromList.Start(objectList);
                            prod = (Product) ret;
                            
                        }catch(NullReferenceException ex){
                            Log.Verbose("Returned null from Product Selection", ex, ex.Message);
                            Console.WriteLine("Cancelled Product Selection");
                            Console.WriteLine("Press Any Key to Continue ...");
                            Console.ReadKey();
                            break;
                        }catch(Exception ex){
                            Log.Error(ex, ex.Message);
                            break;
                        }
                        //Get Number for stock
                        str = _validate.ValidationPrompt("Enter Initial number of Products in Stock", ValidationService.ValidatePositiveInt);
                        int stock = int.Parse(str);
                        // Add Product to Inventory
                        try{
                            _services.AddProductToInventory(_location,prod, stock);
                            Console.WriteLine("Product Added");
                        }catch(Exception ex){
                            Log.Warning(ex.Message);
                            Console.WriteLine(ex.Message);
                        }
                        Console.WriteLine("Press Any Key to Continue ...");
                        Console.ReadKey();
                    break;
                    default:
                        Console.WriteLine("Choose valid option");
                    break;

                }
            } while(repeat);
        }
    }
}