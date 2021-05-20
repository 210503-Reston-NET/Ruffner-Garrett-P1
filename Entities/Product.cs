using System;
using System.Collections.Generic;

#nullable disable

namespace Data.Entities
{
    public partial class Product
    {
        public Product()
        {
            InventoryItems = new HashSet<InventoryItem>();
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double? Price { get; set; }

        public virtual ICollection<InventoryItem> InventoryItems { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
