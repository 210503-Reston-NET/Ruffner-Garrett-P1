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
        public Item(Product product, int quantity)
        {
            this.Product = product;
            this.Quantity = quantity;
        }
        public Item(Product product, int quantity, int id):this(product,quantity)
        {
            this.ID = id;
        }
        public override string ToString()
        {
            return String.Format("{0} Quantity: {1}",Product.ToString(), Quantity);
        }
        public Product Product { get; set; }
        public int ID { get; set; }

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