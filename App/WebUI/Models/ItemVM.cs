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

        public int ProductID { get; set; }
        public ProductVM Product { get; set; }
        public int ItemID { get; set; }

        public int Quantity{ get; set; } 
        
    }
}