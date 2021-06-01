using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        public Location(){
            InventoryItems  = new List<InventoryItem>();
        }
        
         public Location(string locationName, string address, Guid managerId)
        {
            this.Address = address;
            this.LocationName = locationName;
            this.UserId = managerId;
            InventoryItems  = new List<InventoryItem>();
        }
        public Location(string locationName, string address, Guid managerId, List<InventoryItem> inventory, int id )
        {
            this.Address = address;
            this.LocationName = locationName;
            this.InventoryItems  = inventory;
            this.UserId = managerId;
            this.LocationID = id;
        }
        public override string ToString()
        {
            return String.Format("{0} Address: {1}",this.LocationName,this.Address);
        }
        [RequiredAttribute]
        public int LocationID { get; set; }
        [RequiredAttribute]
        [DisplayNameAttribute("Location Address")]
        public string Address { get; set; }
        [RequiredAttribute]
        [DisplayNameAttribute("Location Name")]
        public string LocationName { get; set; }
        [RequiredAttribute]
        public List<InventoryItem> InventoryItems { get; set; }
        [RequiredAttribute]
        public Guid UserId { get; set;} 
    }
}