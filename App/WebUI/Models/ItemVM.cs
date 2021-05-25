using StoreModels;
namespace WebUI.Models
{
    public class ItemVM{
        public ItemVM()
        {
 
        }
        public ItemVM(Item item)
        {
            ProductID = item.ProductID;
            Product = new ProductVM(item.Product);
            ItemID = item.ItemID;
            Quantity = item.Quantity;
        }

         public ItemVM(Item item, int LocationID): this(item)
        {
            // ProductID = item.ProductID;
            // Product = new ProductVM(item.Product);
            // ItemID = item.ItemID;
            // Quantity = item.Quantity;
            this.LocationID = LocationID;
            

        }

        public int ProductID { get; set; }
        public ProductVM Product { get; set; }
        public int ItemID { get; set; }

        public int Quantity{ get; set; } 

        public Location location { get; set; }

        public int LocationID{ get; set; }
        
    }
}