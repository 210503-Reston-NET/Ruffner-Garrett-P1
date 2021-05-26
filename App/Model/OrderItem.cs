namespace StoreModels
{
    public class OrderItem
    {
        public OrderItem(){

        }
        public int OrderID { get; set; }
        public Order Order {get; set;}
        public int ProductID {get; set; }
        public Product Product {get; set;}

        public int Quantity {get; set;}
    }
}