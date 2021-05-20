using System;
using System.Collections.Generic;

namespace StoreModels
{
    /// <summary>
    /// This class should contain all the fields and properties that define a customer order. 
    /// </summary>
    public class Order
    {
        public readonly DateTime Date;
        private double _total;

        public Order(int CustomerID, int LocationID, int OrderID){
            this.CustomerID = CustomerID;
            this.LocationID = LocationID;
            this.OrderID = OrderID;
        }

        public Order(Customer customer, Location location, List<Item> items)
        {
            this.Customer = customer;
            this.Location = location;
            this.OrderItems = items;
            this.Date = DateTime.Now;
            CalculateTotal();
        }
        public Order(Customer customer, Location location, List<Item> items, DateTime date)
        {
            this.Customer = customer;
            this.Location = location;
            this.OrderItems = items;
            this.Date = date;
            CalculateTotal();
        }
         public Order(Customer customer, Location location, List<Item> items, DateTime date, int id): this(customer,location,items,date)
        {
            this.OrderID = id;
        }
        public override string ToString()
        {
            return String.Format("Customer: {0} Location: {1}\n\tOn: {2} Total:{3} ", this.Customer,this.Location,this.Date,this._total);
        }

        public int OrderID { get; set; }
        public int CustomerID {get ;set;}
        public Customer Customer { get; set; }
        public int LocationID { get; set; }
        public Location Location { get; set; }
        public List<Item> OrderItems { get; set; }

        public double Total { get=> _total; set{CalculateTotal();}}
        
        private void CalculateTotal()
        {
            _total = 0;
            foreach (var Item in this.OrderItems)
            {
                //fix this
                _total += 2 * Item.Quantity;
            }
        }
    }
}