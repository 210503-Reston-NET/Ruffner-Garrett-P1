using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoreModels
{
    public class OrderItem
    {
        [Range(0,int.MaxValue)]
        private int _quantity;
        public OrderItem(){

        }
        public int OrderID { get; set; }
        public Order Order {get; set;}
        public int ProductID {get; set; }
        public Product Product {get; set;}

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