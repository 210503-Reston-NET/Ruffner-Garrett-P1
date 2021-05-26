using System;
using System.ComponentModel;

namespace StoreModels
{

    /// <summary>
    /// This is a join table for locaitons and items
    /// </summary>
    public class InventoryItem
    {
        private int _quantity;

        public InventoryItem(){

      }

        public int LocationID{get;set;}

        [DisplayName("Location")]
        public Location location {get; set;}
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [DisplayName("Quantity")]
        public int Quantity 
        {
            get => _quantity; 
            set
            {
                if (value < 0){
                    throw new Exception("Quantity cannot be < 0");
                }else{
                    _quantity = value;
                }
            } 
        }
    }
}