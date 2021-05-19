using System.Runtime.CompilerServices;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Models =StoreModels;
using Entity = Data.Entities;
using System;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using Npgsql;

namespace Data
{
    public class RepoDB : IRepository
    {
        private Entity.p0Context _context;
        public RepoDB(Entity.p0Context context)
        {
            _context = context;
        }
        private IDbContextTransaction _transaction;
        public void AddCustomer(Models.Customer customer)
        {
            _context.Customers.Add(
                new Entity.Customer
                {
                    Name = customer.Name,
                    Address = customer.Address,
                    Email = customer.Email.Address
                }
            );
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
        }

        public void AddLocation(Models.Location location)
        {
            _context.Locations.Add(
                new Entity.Location
                {
                   LocationName = location.LocationName,
                   Address = location.Address
                }
            );
           _context.SaveChanges();
           _context.ChangeTracker.Clear();
        }

        public void AddProduct(Models.Product product)
        {
            _context.Products.Add(
               new Entity.Product
               {
                    Name = product.ProductName,
                    Price = product.Price
               }
            );
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
        }

        public void AddProductToInventory(Models.Location location, Models.Item item)
        {
            _context.InventoryItems.Add( 
                new Entity.InventoryItem
                {
                    Location = GetLocation(location),
                    Product = GetProduct(item.Product),
                    Quantity = item.Quantity
                }
           );
           _context.SaveChanges();
           _context.ChangeTracker.Clear();
        }

        public List<Models.Customer> GetAllCustomers()
        {
            return _context.Customers.Select(
                customer => new Models.Customer(customer.Name, customer.Address, new System.Net.Mail.MailAddress(customer.Email), customer.Id)
            ).ToList();
        }

        public List<Models.Location> GetAllLocations()
        {
            //WHAT HAVE I CREATED
            return _context.Locations.Select(
                location => new Models.Location(
                    location.LocationName, 
                    location.Address, 
                    location.InventoryItems.Select( 
                        i => new Models.Item(
                            new Models.Product(
                                i.Product.Name, 
                                (double) i.Product.Price
                            ),
                            (int) i.Quantity
                        )
                    ).ToList()
                )
            ).ToList();
        }

        public List<Models.Product> GetAllProducts()
        {
            return _context.Products.Select(
                product => new Models.Product(product.Name, (double) product.Price)
            ).ToList();
        }

        public List<Models.Order> GetOrders(Models.Customer customer, bool price, bool asc)
        {
            //OH GOD ITS SO GROSS
            //SOMEONE HELP ME FIND A BETTER WAY
            //Had to use client side Evelaution for where clause            
            List<Models.Order> mOrders=  _context.Orders.Select(
                order => new Models.Order(
                   new Models.Customer(order.Customer.Name, order.Customer.Address, new System.Net.Mail.MailAddress(order.Customer.Email), order.Customer.Id),
                   new Models.Location(order.Location.LocationName, order.Location.Address),
                   order.OrderItems.Select(
                       i => new Models.Item(
                           new Models.Product(
                               i.Product.Name,
                               (double) i.Product.Price),
                               (int) i.Quantity)).ToList(),
                (DateTime) order.Date)
            ).AsEnumerable().Where(order => order.Customer.Name == customer.Name).ToList();

            Func<Models.Order, double> orderbyprice = order => order.Total;
            Func<Models.Order, DateTime> orderbydate = order => order._date;
            IOrderedEnumerable<StoreModels.Order> temp = null;

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

        public List<Models.Order> GetOrders(Models.Location location, bool price, bool asc)
        {
            List<Models.Order> mOrders=  _context.Orders.Select(
                order => new Models.Order(
                   new Models.Customer(order.Customer.Name, order.Customer.Address, new System.Net.Mail.MailAddress(order.Customer.Email), order.Customer.Id),
                   new Models.Location(order.Location.LocationName, order.Location.Address),
                   order.OrderItems.Select(
                       i => new Models.Item(
                           new Models.Product(
                               i.Product.Name,
                               (double) i.Product.Price),
                               (int) i.Quantity)).ToList(),
                (DateTime) order.Date)
            ).AsEnumerable().Where(order => order.Location.LocationName == location.LocationName).ToList();

            Func<Models.Order, double> orderbyprice = order => order.Total;
            Func<Models.Order, DateTime> orderbydate = order => order._date;

            IOrderedEnumerable<StoreModels.Order> temp = null;
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
        
       
        public void PlaceOrder(Models.Order mOrder)
        {  
            List<Entity.OrderItem> items = new List<Entity.OrderItem>{};
            mOrder.Items.ForEach(item => 
                items.Add(
                    new Entity.OrderItem
                    {
                        // OrderId = eOrder.Id,
                        Product = GetProduct(item.Product),
                        Quantity = item.Quantity,
                    })
            );         
            //First Create order
            Entity.Order eOrder=  new Entity.Order
            {
                Customer = GetCustomer(mOrder.Customer),
                Location = GetLocation(mOrder.Location),
                Date = mOrder._date,
                Total = mOrder.Total,
                OrderItems = items
            };
            
            try{
            _context.Orders.Add(eOrder);
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
        
        private Entity.Location GetLocation(Models.Location mLocation)
        {
            Entity.Location found =  _context.Locations.FirstOrDefault( o => (o.LocationName == mLocation.LocationName) && (o.Address == mLocation.Address));
            return found;
        }
        private Entity.Customer GetCustomer(Models.Customer mCustomer)
        {
            Entity.Customer found =  _context.Customers.FirstOrDefault( o => o.Id == mCustomer.ID);
            return found;
        }
        private Entity.Product GetProduct(Models.Product mProduct)
        {
            Entity.Product found = _context.Products.FirstOrDefault(o => (o.Name == mProduct.ProductName)&& (o.Price == mProduct.Price));
            return found;
        }
        private Entity.InventoryItem GetInventoryItem(Models.Item item, Entity.Location eLocation)
        {
            Entity.InventoryItem found = _context.InventoryItems.FirstOrDefault(o=> (o.Product.Name == item.Product.ProductName) && (o.LocationId == eLocation.Id));
            return found;
        }

        public void UpdateInventoryItem(Models.Location location, Models.Item item)
        {
            Entity.Location eLocation = GetLocation(location);
            Entity.InventoryItem eItem = GetInventoryItem(item, eLocation);
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
    }
}