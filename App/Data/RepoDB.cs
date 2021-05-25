using System.Data;
using System.Collections.Generic;
using System.Linq;
using StoreModels;
using System;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace Data
{
    public class RepoDB : IRepository
    {
        private StoreDBContext _context;
        public RepoDB(StoreDBContext context)
        {
            _context = context;
        }
        private IDbContextTransaction _transaction;
        // public void AddCustomer(Customer customer)
        // {
        //     _context.Customers.Add(
        //         customer
        //     );
        //     _context.SaveChanges();
        //     _context.ChangeTracker.Clear();
        // }

        public void AddLocation(Location location)
        {
            Log.Debug("Location Id before db add: {0}", location.LocationID);
            _context.Locations.Add(
                location
            );
           _context.SaveChanges();
           
           _context.Entry(location).GetDatabaseValues();
            Log.Debug("Location Id after db add: {0}", location.LocationID);
        //    _context.ChangeTracker.Clear();
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(
                product
            //    new Entity.Product
            //    {
            //         Name = product.ProductName,
            //         Price = product.Price
            //    }
            );
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
        }

        public void AddProductToInventory(Location location, Item item)
        {
            location.InventoryItems.Add(item);
            _context.InventoryItems.Add( 
                item
                // location.InventoryItems.Add(item)
                // new Entity.InventoryItem
                // {
                //     Location = GetLocation(location),
                //     Product = GetProduct(item.Product),
                //     Quantity = item.Quantity
                // }
           );
           _context.SaveChanges();
           _context.ChangeTracker.Clear();
        }

        public List<ApplicationUser> GetAllCustomers()
        {

            return _context.Users.Select(user => user).ToList();
            // return _context.AspNetUsers.Select(
            //     // customer => new ApplicationUser(customer.Name, customer.Address, customer.Email, customer.CustomerID)
            // ).ToList();
        }

        public List<Location> GetAllLocations()
        {
            //WHAT HAVE I CREATED
            return _context.Locations.Select(location => location).ToList();//new Location(location.LocationName,location.Address, location.UserId, location.InventoryItems, location.LocationID)).ToList();
            //     location => new Location(
            //         location.LocationName, 
            //         location.Address, 
            //         location.InventoryItems.Select( 
            //             i => new Item(
            //                 new  Product(
            //                     i.Product.Name, 
            //                     (double) i.Product.Price
            //                 ),
            //                 (int) i.Quantity
            //             )
            //         ).ToList()
            //     )
            // ).ToList();
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.Select(
                product => new Product(product.Name, (double) product.Price)
            ).ToList();
        }

        public List<Order> GetOrders(ApplicationUser customer, bool price, bool asc)
        {
            //OH GOD ITS SO GROSS
            //SOMEONE HELP ME FIND A BETTER WAY
            //Had to use client side Evelaution for where clause            
            List<Order> mOrders=  _context.Orders.Select(
                 order => order
                //    new Customer(order.Customer.Name, order.Customer.Address,order.Customer.Email, order.Customer.ID),
                //    new Location(order.Location.LocationName, order.Location.Address),
                //    order.Items.Select(
                //        i => new Item(
                //            new Product(
                //                i.Product.Name,
                //                (double) i.Product.Price),
                //                (int) i.Quantity)).ToList(),
                // (DateTime) order.Date)
            ).AsEnumerable().Where(order => order.Customer.Id == customer.Id).ToList();

            Func<Order, double> orderbyprice = order => order.Total;
            Func<Order, DateTime> orderbydate = order => order.Date;
            IOrderedEnumerable<Order> temp = null;

            if(price){
                //order by total
              temp =  mOrders.OrderBy(orderbyprice);
            }else{
                //order by date
              temp = mOrders.OrderBy(orderbydate);
            }
            //Already in ascending order by default. Either reverse it or don't
            if(!asc){
                mOrders = temp.Reverse().ToList();
            }else{
                mOrders = temp.ToList();
            }            
            //return mOrders after results of query have been sorted
            return mOrders;         
        }

        public List<Order> GetOrders(Location location, bool price, bool asc)
        {
            List<Order> mOrders=  _context.Orders.Select(
                order => order
               
                //    new Customer(order.Customer.Name, order.Customer.Address, order.Customer.Email, order.Customer.ID),
                //    new Location(order.Location.LocationName, order.Location.Address),
                //    order.Items.Select(
                //        i => new Item(
                //            new Product(
                //                i.Product.Name,
                //                (double) i.Product.Price),
                //                (int) i.Quantity)).ToList(),
                // (DateTime) order.Date)
            ).AsEnumerable().Where(order => order.Location.LocationID == location.LocationID).ToList();

            Func<Order, double> orderbyprice = order => order.Total;
            Func<Order, DateTime> orderbydate = order => order.Date;

            IOrderedEnumerable<Order> temp = null;
            if(price){
                //order by total
              temp =  mOrders.OrderBy(orderbyprice);
            }else{
                //order by date
              temp = mOrders.OrderBy(orderbydate);
            }
            //Already in ascending order by default. Either reverse it or don't
            if(!asc){
                mOrders = temp.Reverse().ToList();
            }else{
                mOrders = temp.ToList();
            }            
            //return mOrders after results of query have been sorted
            return mOrders;
        }
        
       
        public void PlaceOrder(Order mOrder)
        {  
            // List<Item> items = new List<Item>{};
            // mOrder.Items.ForEach(item => 
            //     items.Add(
            //         new Item
            //         {
            //             // OrderId = eOrder.Id,
            //             Product = GetProduct(item.Product),
            //             Quantity = item.Quantity,
            //         })
            // );         
            //First Create order
            // Order eOrder=  new Order
            // {
            //     Customer = GetCustomer(mOrder.Customer),
            //     Location = GetLocation(mOrder.Location),
            //     Date = mOrder.Date,
            //     Total = mOrder.Total,
            //     OrderItems = items
            // };
            
            try{
            _context.Orders.Add(mOrder);
            //Save Order to DB so that OrderItems entries have an ID to Reverence in the db

            //This can probably be done with the ef-core change tracker in the future
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            }catch(Exception ex){
                Log.Error("Could not add order to db {0}\n {1}", ex.StackTrace, ex.Message);
                throw new Exception("Order Failed");
            }

            // // Add order Items for the order to the table
            // mOrder.Items.ForEach(item => _context.OrderItems.Add(
            // new Entity.OrderItem
            // {
            //     OrderId = eOrder.Id,
            //     Product = GetProduct(item.Product),
            //     Quantity = item.Quantity,
            // }));

            // _context.SaveChanges();
        }
        
        private Location GetLocation(Location mLocation)
        {
            Location found =  _context.Locations.FirstOrDefault( o => (o.LocationID == mLocation.LocationID));
            return found;
        }
        private ApplicationUser GetCustomer(ApplicationUser mCustomer)
        {
            ApplicationUser found =  _context.Users.FirstOrDefault( o => o.Id == mCustomer.Id);
            return found;
        }
        private Product GetProduct(Product mProduct)
        {
            Product found = _context.Products.FirstOrDefault(o => (o.ProductID == mProduct.ProductID));
            return found;
        }
        private Item GetInventoryItem(Item item, Location eLocation)
        {
            Item found = _context.InventoryItems.FirstOrDefault(o=> (o.ItemID == item.ItemID));
            return found;
        }

        public void UpdateInventoryItem(Location location, Item item)
        {
            Location eLocation = GetLocation(location);
            Item eItem = GetInventoryItem(item, eLocation);
            eItem.Quantity = item.Quantity;
            var thing =  _context.InventoryItems.Update(eItem);            
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
        }

        public void StartTransaction()
        {
          _transaction = _context.Database.BeginTransaction();
        }

        public void EndTransaction(bool success)
        {
            if(success){
                _transaction.Commit();
                Log.Information("Transaction Commited Successfully");
            }else{
                _transaction.Rollback();
                Log.Error("Transaction Failed. Rolled back.");
            }
        }

        public Location GetLocationById(int LocationID)
        {
            Log.Verbose("Location ID {0}", LocationID);
            Location found =  _context.Locations.FirstOrDefault( o => (o.LocationID == LocationID));
            return found;
        }
    }
}