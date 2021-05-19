using System;
namespace StoreModels
{
    //This class should contain all necessary fields to define a product.
    public class Product
    {
        private double _price;
        private string _productName;
        public Product(string productName, double price)
        {
            this.ProductName = productName;
            this.Price = price;
        }
        public Product(string productName, double price, int id) : this(productName, price)
        {
            this.ID = id;
        }
        public override string ToString()
        {
            return String.Format("{0}, ${1}",this.ProductName, this.Price);
        }

        public int ID { get; set; }
        public string ProductName 
        { 
            get => _productName; 
            set{
                if(String.IsNullOrWhiteSpace(value)){
                    throw new Exception("Given bad value for Product Name");
                }else{
                    _productName= value;
                }
            }
        }
        public double Price 
        { 
            get => _price; 
            set{
                if (value < 0){
                    throw new Exception("Price cannot be < 0");
                }else{
                    _price = value;
                }
            }
        }
    }
}