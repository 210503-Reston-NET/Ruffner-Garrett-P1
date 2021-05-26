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
            _context.Products.Add(product);
            _context.SaveChanges();
            _context.Entry(product).GetDatabaseValues();
            
        }

        public void AddProductToInventory(Location location, InventoryItem item)
        {
            _context.InventoryItems.Add(item);
            _context.Locations.Update(location);
            _context.SaveChanges();
        }

        public List<ApplicationUser> GetAllCustomers()
        {
            return _context.Users.Select(user => user).ToList();
        }

        public List<Location> GetAllLocations()
        {
            //WHAT HAVE I CREATED
            var l = _context.Locations.Select(
                location =>new Location(){
                    LocationID = location.LocationID,
                    LocationName = location.LocationName,
                    Address = location.Address,
                    UserId = location.UserId,
                    InventoryItems = _context.InventoryItems.Select(
                        i => new InventoryItem(){
                            LocationID = i.LocationID,
                            location = i.location,
                            ProductID = i.ProductID,
                            Product = i.Product,
                            Quantity = i.Quantity
                        }).Where(i => i.LocationID == location.LocationID).ToList()
                    }).ToList();
            return l;
            
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.Select(
                product => product
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
            Log.Verbose("Retrieving Locaiton by Id: {0}", LocationID);
            var locations = this.GetAllLocations();
            //locations.ForEach(l => l.InventoryItems.ForEach(i => Log.Verbose("id: {0} Quantity: {1}",i.ProductID,  i.Quantity)));
            Location found =  locations.Where(l => l.LocationID == LocationID).FirstOrDefault();
            Log.Verbose("Location Found {0}", found.LocationName);
            return found;
        }

        public Product GetProductById(int ProductID)
        {
           Product found = _context.Products.FirstOrDefault(o => o.ProductID == ProductID);
           return found;
        }

        public void UpdateInventoryItem(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            _context.Entry(item).GetDatabaseValues();
            _context.SaveChanges();
        }
    }
}