using System.Linq;
using System.Collections.Generic;
using StoreModels;
namespace WebUI.Models
{
    public class LocationVM
    {
        public LocationVM(){
            
        }
        public LocationVM(Location location){
            LocationID = location.LocationID;
            Address = location.Address;
            LocationName = location.LocationName;
            InventoryItems = new List<ItemVM>(location.InventoryItems.Select(Item => new ItemVM(Item)));
        }
        public int LocationID { get; set; }
        public string Address { get; set; }
        public string LocationName { get; set; }

        public List<ItemVM> InventoryItems { get; set; }
    }
}