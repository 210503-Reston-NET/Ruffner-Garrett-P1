using System.Net.Mail;
using System;
using StoreModels;
using Data;
using System.Collections.Generic;
using Serilog;
using System.ComponentModel;

namespace Service
{
    public class Services : IServices
    {
        private IRepository _repo;
        private IEmailService _emailService;
       
        public Services(IRepository repo, IEmailService emailService)
        {
            _repo = repo;
            _emailService = emailService;
        }

        // public void AddCustomer(string name, string address, string email)
        // {   
        //     Log.Debug("Adding new customer: {0}, {1}, {2}", name, address, email);
        //      MailAddress cEmail = new MailAddress(email);
        //      Customer newCustomer = new Customer(name,address, email);
        //     try{
        //         cEmail = new MailAddress(email);
        //         newCustomer = new Customer(name, address, email);
        //     }catch(Exception ex){
        //         Log.Error("Could not create new customer, {0}\n{1}",ex.Message, ex.StackTrace);
        //     }

        //     if(CheckForCustomer(newCustomer, _repo.GetAllCustomers()))
        //     {
        //         //customer already exists
        //         Log.Debug("Customer {0} Already exists",newCustomer.Name);
        //         throw new Exception("Customer Already Exits");
        //     }
        //     try{
        //         _repo.AddCustomer(newCustomer);
        //         _emailService.SendWelcomeEmail(newCustomer);
        //     }catch(Exception ex){

        //         Log.Error("Failed to Add Customer. {0}\n{1}",ex.Message, ex.StackTrace);
        //         throw new Exception("Failed to Add Customer");
        //     }
        // }
        

        public Location AddLocation(string name, string address, Guid ManagerId)
        {
            Location newLocation = new Location(name, address, ManagerId);
            if(CheckForLocations(newLocation, _repo.GetAllLocations()))
            {
                //customer already exists
                Log.Debug("Location {0} Already exists",newLocation.LocationName);
                throw new Exception("Location Already Exits");
            }
            try{
                _repo.AddLocation(newLocation);
            }catch(Exception ex){
                Log.Error("Failed to Add Location. {0}",ex.Message);
                throw new Exception("Failed to Add Location");
              
            }
            return newLocation;
        }

        public void AddProduct(string productName, double productPrice)
        {
            Product newProduct = new Product(productName, productPrice);
            if(CheckForProduct(newProduct, _repo.GetAllProducts()))
            {
                //customer already exists
                Log.Debug("Product {0} Already exists",newProduct.Name);
                throw new Exception("Product Already Exits");
            }
            try{
                _repo.AddProduct(newProduct);
            }catch(Exception ex){
                Log.Error("Failed to Add Product. {0}",ex.Message);
                throw new Exception("Failed to Add Product");
            }
        }

        public void AddProductToInventory(int LocationID, int ProductID, int stock)
        {           
            //Check if Location + product Exist
            Location location = _repo.GetLocationById(LocationID);
            if (location == null) throw new Exception("Location Does not exist");
            Product product = _repo.GetProductById(ProductID);
            if (product == null) throw new Exception("Product Does not exist");
            // Create new Item for inventory
            InventoryItem ii = new InventoryItem(){LocationID = LocationID, ProductID = ProductID, Quantity = stock};
            //check if product is int inventory
            List<InventoryItem> InventoryItems = getInventory(LocationID);

            foreach (var item in InventoryItems)
            {
                if(item.ProductID == ProductID){
                    throw new Exception("Item is already included in Locations Inventory");
                }
            }
            //Product is not in inventory

            //Add Item to Inventory
            try{
                Log.Verbose("{0}",product.ProductID);
                Log.Verbose("{0}",location.LocationID);
                Log.Verbose("{0}", product.InventoryItems);
                if(product.InventoryItems == null){
                    product.InventoryItems = new List<InventoryItem>();
                }
                if(location.InventoryItems ==null){
                    location.InventoryItems = new List<InventoryItem>();
                }
                product.InventoryItems.Add(ii);
                location.InventoryItems.Add(ii);
                _repo.AddProductToInventory(location, ii);
            }catch(Exception ex){
                Log.Error("Failed to Add Product To Inventory {0} \n{1}",ex.Message, ex.StackTrace);
                throw new Exception("Failed to Add product to Inventory");
            }
        }

        public List<ApplicationUser> GetAllCustomers()
        {
            Log.Verbose("Retrieveing all Customers From DB");
            List<ApplicationUser> retVal;
            retVal = _repo.GetAllCustomers();
            return retVal;
        }
        public List<Location> GetAllLocations()
        {
            List<Location> retVal;
            retVal = _repo.GetAllLocations();
            return retVal;
        }

        public List<Order> GetOrders(ApplicationUser customer, bool price, bool asc)
        {
           return _repo.GetOrders(customer, price, asc);
        }

        public List<Order> GetOrders(Location location, bool price, bool asc)
        {
            return _repo.GetOrders(location, price, asc);
        }

        public List<Product> GetAllProducts()
        {
           return _repo.GetAllProducts();
        }

        public void PlaceOrder(Order order)
        {
            // Order order = new Order(customer, location, items);
            // //make sure that location has stock then decrease stock
            //     //For each item in items, get relavant item from location and try to decrease stock
            //         //Then call UpdateInventoryItem(Models.Location location, Models.Item item) with each updated item.
            // //This is going to be kinda slow n^2 time :(
            // //Start transaction
            
             _repo.StartTransaction();
            try{
                SellItems(order.LocationID, order.OrderItems);
            }catch(Exception ex){
                Log.Error("Could not update stock From order. Rolling back",ex, ex.Message);
                _repo.EndTransaction(false);
                throw new Exception("Not enough of an Item in stock. Order Failed.");
            }
            Log.Verbose("Adding Order to DB");
            try{
            _repo.PlaceOrder(order);
            _repo.EndTransaction(true);
            //_emailService.SendOrderConfirmationEmail(customer, order);
            }catch(Exception ex )
            {
                _repo.EndTransaction(false);
                Log.Error("Failed to place order\n{0}\n{1}\n{2}", ex, ex.Message, ex.StackTrace);
                throw new Exception("Order Failed");
            }
        }
        private void SellItems(int LocationID, List<OrderItem> OrderItems)
        {
            //get item from inventory then reduce quantity by specified amount
            List<InventoryItem> iis = getInventory(LocationID);
            foreach (OrderItem item in OrderItems)
            {
                InventoryItem i = iis.Find(i => i.ProductID == item.ProductID);
                i.Quantity = i.Quantity-item.Quantity;
                _repo.UpdateInventoryItem(i);
            }
        }

        public ApplicationUser SearchCustomers(string name)
        {
            Log.Verbose("Searching for Customer: {0}",name);         
            List<ApplicationUser> customers = GetAllCustomers();
            
            foreach (ApplicationUser item in customers)
            {
                if(name == item.Name)
                {
                    Log.Verbose("Found Customer {0}",item.Name);
                    return item;
                }
            }
            Log.Verbose("Customer: {name} not found", name);
            throw new Exception("Customer not found");
                        
        }

        public void updateItemInStock(InventoryItem item)
        {   
            //Log.Debug("Updating stock of {0} at {1} Qunatity:{2}",item.Product.Name,location.LocationName, amount);
            //item.ChangeQuantity(amount);
            try{
                _repo.UpdateInventoryItem(item);
                //_repo.UpdateLocation(location);
            }catch(Exception ex){
                Log.Error("Could not update inventory at Location",ex, ex.Message);
                //item.ChangeQuantity(-amount);
            }
        }

        private bool CheckForCustomer(Customer customer, List<Customer> Customers)
        {

            foreach (Customer item in Customers)
            {
                if(customer.Name == item.Name && customer.Address == item.Address && customer.Email == item.Email)
                {
                    return true;
                }
            }
            return false;
        }
        private bool CheckForLocations(Location location, List<Location> locations)
        {

            foreach (Location item in locations)
            {
                if((location.LocationName == item.LocationName)&&(location.Address == location.Address))
                {
                    return true;
                }
            }

            return false;
        }
        private bool CheckForProduct(Product product, List<Product> products)
        {
            foreach (Product item in products)
            {
                if(item.Name == product.Name)
                {
                    return true;
                }
            }
            return false;
        }

        public double CalculateOrderTotal(List<Item> items)
        {
            // double total = 0;
            // foreach(Item item in items)
            // {
            //    total += item.Product.Price * item.Quantity;
            // }
            // return total;
            return 10;
        }

        public List<InventoryItem> getInventory(int LocationID)
        {
           Log.Verbose("Retreiving Inventory of Location {0}",LocationID);
           Location l =  _repo.GetLocationById(LocationID);
         
           return l.InventoryItems;
        }

    }
}