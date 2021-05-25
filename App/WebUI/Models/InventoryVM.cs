using System.Collections.Generic;
using StoreModels;

namespace WebUI.Models
{
    public class InventoryVM
    {
        public InventoryVM(){

        }
        public InventoryVM(Location location){
            Inventory = new List<ItemVM>();
            //Inventory = location.InventoryItems.ForEach(item => Inventory.Add(new ItemVM(item)));
        }

        private void addStuff(List<Item> items){
            items.ForEach(item => this.Inventory.Add(new ItemVM(item)));
        }
        public List<ItemVM> Inventory;
    }
}