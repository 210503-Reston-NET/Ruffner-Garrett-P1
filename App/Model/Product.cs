using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoreModels
{
    //This class should contain all necessary fields to define a product.
    public class Product
    {
        private double _price;
        private string _name;
        public Product(){
            
        }
        public Product(string name, double price)
        {
            this.Name = name;
            this.Price = price;
        }
        // public Product(string name, double price, int id) : this(name, price)
        // {
        //     this.ProductID = id;
        // }
        public override string ToString()
        {
            return String.Format("{0}, ${1}",this.Name, this.Price);
        }

        public int ProductID { get; set; }

        [DisplayName("Product Name")]
        public string Name 
        { 
            get => _name; 
            set{
                if(String.IsNullOrWhiteSpace(value)){
                    throw new Exception("Given bad value for Product Name");
                }else{
                    _name= value;
                }
            }
        }
        public List<OrderItem> OrderItems{get; set;}
        public List<InventoryItem> InventoryItems{get; set;}

        [Range(0,double.MaxValue)]
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