using System;
using System.Collections.Generic;

#nullable disable

namespace Data.Entities
{
    public partial class Location
    {
        public Location()
        {
            InventoryItems = new HashSet<InventoryItem>();
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string Address { get; set; }
        public string LocationName { get; set; }

        public virtual ICollection<InventoryItem> InventoryItems { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
