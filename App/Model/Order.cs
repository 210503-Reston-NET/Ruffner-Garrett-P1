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

        public Order(Guid CustomerID, int LocationID, int OrderID){
            this.CustomerID = CustomerID;
            this.LocationID = LocationID;
            this.OrderID = OrderID;
        }
        public Order(){

        }

         public Order(ApplicationUser customer, Location location, List<OrderItem> items, DateTime date, int id)
        {
            this.OrderID = id;
            this.Customer = customer;
            this.Location = location;
            this.OrderItems = items;
            this.Date = date;
            CalculateTotal();
        }
        public override string ToString()
        {
            return String.Format("Customer: {0} Location: {1}\n\tOn: {2} Total:{3} ", this.Customer.Name,this.Location,this.Date,this._total);
        }

        public int OrderID { get; set; }
        public Guid CustomerID {get ;set;}
        public ApplicationUser Customer { get; set; }
        public int LocationID { get; set; }
        public Location Location { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public double Total { get=> _total; set{CalculateTotal();}}
        
        private void CalculateTotal()
        {
            _total = 0;
            foreach (var Item in this.OrderItems)
            {
                _total += Item.Product.Price * Item.Quantity;
            }
        }
    }
}