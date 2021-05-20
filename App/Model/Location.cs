using System;
using System.Globalization;
using System.Collections.Generic;
namespace StoreModels
{
    /// <summary>
    /// This class should contain all the fields and properties that define a store location.
    /// </summary>
    public class Location
    {
        public Location(string locationName, string address)
        {
            this.Address = address;
            this.LocationName = locationName;
            InventoryItems  = new List<Item>();
        }
        public Location(string locationName, string address, List<Item> inventory)
        {
            this.Address = address;
            this.LocationName = locationName;
            this.InventoryItems  = inventory;
        }
        public Location(string locationName, string address, List<Item> inventory, int id ): this(locationName, address, inventory)
        {
            this.LocationID = id;
        }
        public override string ToString()
        {
            return String.Format("{0} Address: {1}",this.LocationName,this.Address);
        }
        public int LocationID { get; set; }
        public string Address { get; set; }
        public string LocationName { get; set; }

        public List<Item> InventoryItems { get; set; }
    }
}