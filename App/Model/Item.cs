using System.Net.Http.Headers;
using System;

namespace StoreModels
{

    /// <summary>
    /// This data structure models a product and its quantity. The quantity was separated from the product as it could vary from orders and locations.  
    /// </summary>
    public class Item
    {
        private int _quantity;
        public Item(int ProductID, int quantity)
        {
            this.ProductID = ProductID;
            this.Quantity = quantity;
        }
        public Item(int ProductID, int quantity, int ItemId):this(ProductID,quantity)
        {
            this.ItemID = ItemId;
        }
        public override string ToString()
        {
            return String.Format("{0} Quantity: {1}",ProductID, Quantity);
        }
        //PK
        public int ItemID { get; set; }
        //FK
        public int ProductID { get; set; }
        public Product Product { get; set; }
       

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
        public void ChangeQuantity(int num)
        {
            this.Quantity = this.Quantity + num;
        }

    }
}