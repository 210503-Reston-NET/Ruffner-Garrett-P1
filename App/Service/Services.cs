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
        private readonly IRepository _repo;
        private readonly IEmailService _emailService;
       
        public Services(IRepository repo, IEmailService emailService)
        {
            _repo = repo;
            _emailService = emailService;
        }
        
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
                product.InventoryItems.Add(ii);
                _repo.AddProductToInventory(location, ii);
                Log.Debug("Product added to inventory successfully: productID{0} locationID{1}", ii.ProductID, ii.LocationID);
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
                order.Date = DateTime.Now;
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

        public void updateItemInStock(InventoryItem item)
        {   
            try{
                _repo.UpdateInventoryItem(item);
            }catch(Exception ex){
                Log.Error("Could not update inventory at Location",ex, ex.Message);
            }
        }
        private bool CheckForLocations(Location location, List<Location> locations)
        {

            foreach (Location item in locations)
            {
                if(location.LocationID == item.LocationID)
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
                if(item.ProductID == product.ProductID)
                {
                    return true;
                }
            }
            return false;
        }

        public List<InventoryItem> getInventory(int LocationID)
        {
           Log.Verbose("Retreiving Inventory of Location {0}",LocationID);
           Location l =  _repo.GetLocationById(LocationID);
         
           return l.InventoryItems;
        }

        public List<Order> GetOrdersByCustomerId(Guid CustomerID)
        {
            return _repo.GetOrdersByCustomerID(CustomerID);
        }
        public List<Order> GetOrdersByLocationId(int LocationID)
        {
            return _repo.GetOrdersByLocationID(LocationID);
        }

        public Order GetOrder(int OrderID)
        {
            try{
                return _repo.GetOrderByID(OrderID);
            }catch(Exception ex){
                Log.Error("Could not get Order with ID: {0}\n{1}", OrderID,ex.Message);
                return null;
            }

        }
    }
}