using StoreModels;
namespace WebUI.Models
{
    public class ProductVM
    {
        public ProductVM()
        {

        }
        public ProductVM(Product product)
        {
            Name = product.Name;
            Price = product.Price;
        }

        public string Name {get; set;}
        public double Price {get; set;}
    }
}