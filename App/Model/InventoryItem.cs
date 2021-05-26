using System;

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
        public Location location {get; set;}
        public int ProductID { get; set; }
        public Product Product { get; set; }

        public int Quantity 
        { get; set; }
        //     get => _quantity; 
        //     set
        //     {
        //         if (value < 0){
        //             throw new Exception("Quantity cannot be < 0");
        //         }else{
        //             _quantity = value;
        //         }
        //     } 
        // }
    }
}