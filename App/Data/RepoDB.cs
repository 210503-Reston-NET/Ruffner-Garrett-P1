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
        private readonly StoreDBContext _context;
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
           _context.ChangeTracker.Clear();
        }

        public void AddProduct(Product product)
        {
            _context.ChangeTracker.Clear();
            _context.Products.Add(product);
            _context.SaveChanges();
            _context.Entry(product).GetDatabaseValues();
            
            
        }

        public void AddProductToInventory(Location location, InventoryItem item)
        {
            _context.ChangeTracker.Clear();
            _context.InventoryItems.Add(item);
            _context.Locations.Update(location);
            _context.Entry(item).GetDatabaseValues();
            _context.Entry(location).GetDatabaseValues();
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

        public List<Order> GetOrdersByCustomerID(Guid CustomerID)
        {          
            List<Order> order=  _context.Orders.Select(
                order => new Order(){
                    OrderID = order.OrderID,
                    CustomerID = order.CustomerID,
                    LocationID = order.LocationID,
                    Date = order.Date,
                    Location = order.Location,
                    Customer = order.Customer,
                    OrderItems = _context.OrderItems.Select(
                        orderitem => new OrderItem(){
                            OrderID = orderitem.OrderID,
                            ProductID = orderitem.ProductID,
                            Quantity = orderitem.Quantity,
                            Product = orderitem.Product,
                            Order = orderitem.Order                             
                        }).Where(o => o.OrderID == order.OrderID).ToList(),
                        Total = order.Total
                    
                }
            ).Where(order => order.CustomerID == CustomerID).ToList();
            return order;         
        }

        public List<Order> GetOrders(Location location, bool price, bool asc)
        {
            List<Order> order=  _context.Orders.Select(
                order => order
            ).AsEnumerable().Where(order => order.Location.LocationID == location.LocationID).ToList();

            Func<Order, double> orderbyprice = order => order.Total;
            Func<Order, DateTime> orderbydate = order => order.Date;

            IOrderedEnumerable<Order> temp = null;
            if(price){
                //order by total
              temp =  order.OrderBy(orderbyprice);
            }else{
                //order by date
              temp = order.OrderBy(orderbydate);
            }
            //Already in ascending order by default. Either reverse it or don't
            if(!asc){
                order = temp.Reverse().ToList();
            }else{
                order = temp.ToList();
            }            
            //return order after results of query have been sorted
            return order;
        }
        
       
        public void PlaceOrder(Order mOrder)
        {              
            try{
            _context.Orders.Add(mOrder);
            //Save Order to DB so that OrderItems entries have an ID to Reverence in the db

            //This can probably be done with the ef-core change tracker in the future
            _context.SaveChanges();
            }catch(Exception ex){
                Log.Error("Could not add order to db {0}\n {1}", ex.StackTrace, ex.Message);
                throw new Exception("Order Failed");
            }
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
            Location found =  locations.Where(l => l.LocationID == LocationID).FirstOrDefault();
            Log.Verbose("Location Found {0}", found.LocationName);
            return found;
        }

        public Product GetProductById(int ProductID)
        {
           Product found = _context.Products.Select(
               p => new Product(){
                   Name = p.Name,
                   Price = p.Price,
                   ProductID = p.ProductID,
               }
               ).FirstOrDefault(o => o.ProductID == ProductID);
           return found;
        }

        public void UpdateInventoryItem(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            _context.Entry(item).GetDatabaseValues();
            _context.SaveChanges();
        }

        public Order GetOrderByID(int OrderID)
        {
            var order = _context.Orders.Select(order => new Order()
            {
                OrderID = order.OrderID,
                CustomerID = order.CustomerID,
                Customer = order.Customer,
                LocationID = order.LocationID,
                Location = order.Location,
                Date = order.Date,                
                OrderItems = _context.OrderItems.Select(
                    orderitem => new OrderItem(){
                        OrderID = orderitem.OrderID,
                        ProductID = orderitem.ProductID,
                        Quantity = orderitem.Quantity,
                        Product = orderitem.Product,
                        Order = orderitem.Order                             
                    }).Where(o => o.OrderID == order.OrderID).ToList(),
                Total = order.Total
               
            }).Where(o => o.OrderID == OrderID).FirstOrDefault();

            return order;
        }

        public List<Order> GetOrdersByLocationID(int LocationID){
            List<Order> order=  _context.Orders.Select(
                order => new Order(){
                    OrderID = order.OrderID,
                    CustomerID = order.CustomerID,
                    LocationID = order.LocationID,
                    Date = order.Date,
                    Location = order.Location,
                    Customer = order.Customer,
                    OrderItems = _context.OrderItems.Select(
                        orderitem => new OrderItem(){
                            OrderID = orderitem.OrderID,
                            ProductID = orderitem.ProductID,
                            Quantity = orderitem.Quantity,
                            Product = orderitem.Product,
                            Order = orderitem.Order                             
                        }).Where(o => o.OrderID == order.OrderID).ToList(),
                        Total = order.Total
                    
                }
            ).Where(order => order.LocationID == LocationID).ToList();
            return order;         



        }
    }
}