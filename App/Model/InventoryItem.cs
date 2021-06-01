using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoreModels
{

    /// <summary>
    /// This is a join table for locaitons and items
    /// </summary>
    public class InventoryItem
    {
        [Range(0,int.MaxValue)]
        private int _quantity;

        public InventoryItem(){

      }

        public int LocationID{get;set;}

        [DisplayName("Location")]
        public Location location {get; set;}
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [DisplayName("Quantity")]
        [Range(0,int.MaxValue)]
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