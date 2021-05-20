using System.Linq;
using System;
using Serilog;
using Service;
using StoreModels;
using System.Collections.Generic;
namespace UI
{
    public class MainMenu : IMenu
    {   
        private IService _services;
        private IValidationUI _validate;

        public MainMenu(IService services, IValidationUI validation)
        {
            _services = services;
            _validate = validation;
        }

        public void Start()
        {   
           // _validate = new ValidationUI();
            bool repeat = true;
            do{
                Console.Clear();
                Console.WriteLine("Welcome!");
                Console.WriteLine("Main Menu:");
                Console.WriteLine("[0] Exit");
                Console.WriteLine("[1] Search For Customer");
                Console.WriteLine("[2] List Customers");
                Console.WriteLine("[3] List Locations");
                Console.WriteLine("[4] List Products");
                Console.WriteLine("[5] View Orders");
                Console.WriteLine("[6] Create Order");
                Console.WriteLine("[7] Admin Menu");
                // Console.WriteLine("[8] Select Product");

                
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
                        // Search for Customer
                        SearchForCustomer();
                    break;
                    case "2":
                        //List Customers
                        ListCustomers();
                    break;
                    case "3":
                        //List Locations
                        ListLocations();
                    break;
                    case "4":
                        //List Products
                        ListProducts();
                    break;
                    case "5":
                    // View Orders
                        ViewOrders();
                    break;
                    case "6":
                    // Create Orders
                        CreateNewOrder();

                    break;
                    case "7":
                    //Admin Menu
                    MenuFactory.GetMenu("adminmenu").Start();
                           
                    break;
                    case "8":
                        //Get Product From List
                        try{ 
                            List<Object> products = _services.GetAllProducts().Cast<Object>().ToList<Object>();
                            
                            Object ret = SelectFromList.Start(products);
                            Product prod = (Product) ret;

                            Console.WriteLine("Product selected: {0}", prod.ToString());
                            Console.WriteLine("Press Any Key to Continue ...");
                            Console.ReadKey();

                        }catch(NullReferenceException ex){
                            Log.Verbose("Returned null from Product Selection", ex, ex.Message);
                            Console.WriteLine("Cancelled Product Selection");
                            Console.WriteLine("Press Any Key to Continue ...");
                            Console.ReadKey();
                        }catch(Exception ex){
                            Log.Error(ex, ex.Message);
                        }
                        break;
                    default:
                        Console.WriteLine("Choose valid option");
                    break;

                }
            } while(repeat);
            
        }

        private void SearchForCustomer()
        {
            string str;
            str = _validate.ValidationPrompt("Enter Customer Name", ValidationService.ValidatePersonName);
            Customer target = null;
            try{
                target =  _services.SearchCustomers(str);
                Console.Clear();
                Console.WriteLine("Customer found: {0}", target.ToString());
            }catch(Exception ex){
                Log.Debug(ex.Message);
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Press Any Key to Continue ...");
            Console.ReadKey();
        }

        private void ListCustomers()
        {
            try{
                List<Customer> customers =_services.GetAllCustomers();
                Console.Clear();
                Console.WriteLine("Customers:");
                foreach (Customer customer in customers)
                {
                    Console.WriteLine(customer.ToString());
                }
                Console.WriteLine();  
            }catch(Exception ex){
                Log.Debug(ex.Message);
                Console.WriteLine(ex.Message);
            }                    
            Console.WriteLine("Press Any Key to Continue ...");
            Console.ReadKey();
        }

        private void ListLocations()
        {
            try{
                List<Location> locations =_services.GetAllLocations();
                Console.Clear();
                Console.WriteLine("Locations:");
                foreach (Location location in locations)
                {
                    Console.WriteLine(location.ToString());
                }
                Console.WriteLine();
            }catch(Exception ex){
                Log.Debug(ex.Message);
                Console.WriteLine(ex.Message);
            }                    
            Console.WriteLine("Press Any Key to Continue ...");
            Console.ReadKey();
        }
        private void ListProducts()
        {
             try{
                List<Product> products =_services.GetAllProducts();
                Console.Clear();
                Console.WriteLine("Products:");
                foreach (Product product in products)
                {
                    Console.WriteLine(product.ToString());
                }  
                Console.WriteLine();
            }catch(Exception ex){
                Log.Debug(ex.Message);
                Console.WriteLine(ex.Message);
            }                    
            Console.WriteLine("Press Any Key to Continue ...");
            Console.ReadKey();
        }

        private void ViewOrders()
        {
            //view by location or by customer
            bool repeat = true;
                do
                {
                    Console.Clear();
                    Console.WriteLine("View Orders:");
                    Console.WriteLine("[0] Exit");
                    Console.WriteLine("[1] View By customer");
                    Console.WriteLine("[2] View By Location");
                    String str = Console.ReadLine();
                    switch (str) {
                        case "0":
                           repeat = false;
                        break;
                        case "1":
                            //View by customer
                            //Choose customer
                            ViewByCustomer();
                        break;
                        case "2":
                            //View by Location
                            //Choose location
                            ViewByLocation();
                        break;
                    }
                } while (repeat);
            //order by price asc/desc

            //order by date asc/desc
        }

        private void ViewByCustomer()
        {
            try{ 
                List<Object> objs = _services.GetAllCustomers().Cast<Object>().ToList<Object>();
                
                Object ret = SelectFromList.Start(objs);
                Customer customer = (Customer) ret;

                 //Get input on how to sort list
                bool inpt = true;
                bool price = true;
                bool asc = true;
                string str;
                do{
                    Console.Clear();
                    Console.WriteLine("[0] Sort By Price Ascending");
                    Console.WriteLine("[1] Sort By Price Descending");
                    Console.WriteLine("[2] Sort By Date Ascending");
                    Console.WriteLine("[3] Sort By Date Descending");
                    str = Console.ReadLine();
                    switch(str){
                        case "0":
                            price = true;
                            asc = true;
                            inpt = false;
                        break;
                        case "1":
                            price = true;
                            asc = false;
                            inpt = false;
                        break;
                        case "2":
                            price = false;
                            asc = true;
                            inpt = false;
                        break;
                        case "3":
                            price = false;
                            asc = false;
                            inpt = false;
                        break;
                        default:
                            Console.WriteLine("Invalid entry.");
                        break;
                    }
                }while(inpt);


                //get list of order history
                List<Object> orderList =  _services.GetOrders(customer, price, asc).Cast<Object>().ToList<Object>();
                ret = SelectFromList.Start(orderList);
                Order o = (Order) ret;
                List<Item> items = o.Items;
                items.ForEach(d => Console.WriteLine(d.ToString()));
                Console.WriteLine("Press Any Key to Continue ...");
                Console.ReadKey();

            }catch(NullReferenceException ex){
                Log.Verbose("Returned null from Customer Selection", ex, ex.Message);
                Console.WriteLine("Cancelled Selection");
                Console.WriteLine("Press Any Key to Continue ...");
                Console.ReadKey();
            }catch(Exception ex){
                Log.Error(ex, ex.Message);
            }
        }
        private void ViewByLocation()
        {
            try{ 
                List<Object> objs = _services.GetAllLocations().Cast<Object>().ToList<Object>();
                
                Object ret = SelectFromList.Start(objs);
                Location location = (Location) ret;

                //Get input on how to sort list
                bool inpt = true;
                bool price = true;
                bool asc = true;
                string str;
                do{
                    Console.Clear();
                    Console.WriteLine("[0] Sort By Price Ascending");
                    Console.WriteLine("[1] Sort By Price Descending");
                    Console.WriteLine("[2] Sort By Date Ascending");
                    Console.WriteLine("[3] Sort By Date Descending");
                    str = Console.ReadLine();
                    switch(str){
                        case "0":
                            price = true;
                            asc = true;
                            inpt = false;
                        break;
                        case "1":
                            price = true;
                            asc = false;
                            inpt = false;
                        break;
                        case "2":
                            price = false;
                            asc = true;
                            inpt = false;
                        break;
                        case "3":
                            price = false;
                            asc = false;
                            inpt = false;
                        break;
                        default:
                            Console.WriteLine("Invalid entry.");
                        break;
                    }
                }while(inpt);

                

                //get list of order history
                List<Object> orderList =  _services.GetOrders(location, price, asc).Cast<Object>().ToList<Object>();
                ret = SelectFromList.Start(orderList);
                Order o = (Order) ret;
                List<Item> items = o.Items;
                items.ForEach(d => Console.WriteLine(d.ToString()));
                Console.WriteLine("Press Any Key to Continue ...");
                Console.ReadKey();

            }catch(NullReferenceException ex){
                Log.Verbose("Returned null from Location Selection", ex, ex.Message);
                Console.WriteLine("Cancelled Selection");
                Console.WriteLine("Press Any Key to Continue ...");
                Console.ReadKey();
            }catch(Exception ex){
                Log.Error(ex, ex.Message);
            }
        }
        private void CreateNewOrder()
        {
            //Get Customer
            Customer cust = GetCustomer();
            if (cust == null) return;

            //Get Location
            Location loc = GetLocation();
            if(loc == null) return;
            
            //Choose Items and Quanitity from inventory
            List<Item> itms = GetItems(loc);
            if(itms.Count == 0) return;

            Double total = _services.CalculateOrderTotal(itms);

            Console.WriteLine("Order Total is: {0}", total);
            Console.WriteLine("Press f to complete order\nAny other Key to Cancel ...");
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key.ToString().ToLower()== "f")
            {
                try{
                    _services.PlaceOrder(loc, cust, itms);
                    Console.WriteLine("Order Placed");
                    Console.WriteLine("Press Any Key to Continue ...");
                    Console.ReadKey();
                }catch(Exception ex ){
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press Any Key to Continue ...");
                    Console.ReadKey();
                    Log.Error("Placing An Order Failed", ex, ex.Message, ex.StackTrace);
                }
            }
        }

        private List<Item> GetItems(Location loc)
        {
            //Only allows one item to be selected also no stock checking
            List<Item> selectedItem = new List<Item>();
            string str;
            bool cont = true;
                try{
                    do{
                        Console.Clear();
                        List<Object> objectList = loc.InventoryItems.Cast<Object>().ToList<Object>();
                        
                        Object ret = SelectFromList.Start(objectList);
                        Item itm = (Item) ret;
                        Product p = itm.Product;
                        str = _validate.ValidationPrompt("Enter Quantity to Purchase", ValidationService.ValidatePositiveInt);
                        //make sure that the item isnt already in the order bc db cannot handle it

                        selectedItem.Add(new Item(p, int.Parse(str)));
                        bool innercont = true;
                        do{
                            Console.Clear();
                            Console.WriteLine("[0] Continue With Order");
                            Console.WriteLine("[1] Add Another Item");
                            str = Console.ReadLine();
                            switch(str){
                                case "0":
                                    innercont = false;
                                    cont = false;
                                break;
                                case "1":
                                    innercont = false;
                                break;
                                default:
                                    Console.WriteLine("Invalid entry.");
                                break;
                            }
                        }while(innercont);

                    }while(cont);
                    
                }catch(NullReferenceException ex){
                    Log.Verbose("Returned null from Item Selection", ex, ex.Message);
                    Console.WriteLine("Cancelled Item Selection");
                    Console.WriteLine("Press Any Key to Continue ...");
                    Console.ReadKey();
                    
                }catch(Exception ex){
                    Log.Error(ex, ex.Message);
                    
                }
                return selectedItem;
        }

        private Customer GetCustomer()
        {
            Customer cust = null;
             try{ 
                List<Object> objs = _services.GetAllCustomers().Cast<Object>().ToList<Object>();
                
                Object ret = SelectFromList.Start(objs);
                cust = (Customer) ret;

                Console.Clear();
                Console.WriteLine("Customer selected: {0}", cust.ToString());

            }catch(NullReferenceException ex){
                Log.Verbose("Returned null from Customer Selection", ex, ex.Message);
                Console.WriteLine("Cancelled Selection");
                Console.WriteLine("Press Any Key to Continue ...");
                Console.ReadKey();
                
            }catch(Exception ex){
                Log.Error(ex, ex.Message);
            }
            return cust;
        }
        private Location GetLocation()
        {
            Location loc = null;
            try{ 
                List<Object> objectList = _services.GetAllLocations().Cast<Object>().ToList<Object>();
                
                Object ret = SelectFromList.Start(objectList);
                loc = (Location) ret;
                Console.Clear();
                Console.WriteLine("Location selected: {0}", loc.ToString());
                // Console.WriteLine("Press Any Key to Continue ...");
                // Console.ReadKey();
                

            }catch(NullReferenceException ex){
                Log.Verbose("Returned null from Location Selection", ex, ex.Message);
                Console.WriteLine("Cancelled Location Selection");
                Console.WriteLine("Press Any Key to Continue ...");
                Console.ReadKey();
               
            }catch(Exception ex){
                Log.Error(ex, ex.Message);
              
            }
            return loc;
        }
    }
}