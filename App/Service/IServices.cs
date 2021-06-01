using System;
using System.Collections.Generic;
using StoreModels;
namespace Service
{
    public interface IServices
    {
        public List<ApplicationUser> GetAllCustomers();
        public List<Location> GetAllLocations();
        /// <summary>
        /// Get Products From Repo
        /// </summary>
        /// <returns></returns>
        public List<Product> GetAllProducts();
        /// <summary>
        /// Create and Add a product to Repo
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="productPrice"></param>
        public void AddProduct(string productName, double productPrice);
        /// <summary>
        /// Add Product that already exists in Repo to a Location with initial stock
        /// </summary>
        /// <param name="location"></param>
        /// <param name="product"></param>
        /// <param name="stock"></param>
        public void AddProductToInventory(int LocationID, int ProductID, int stock);
        /// <summary>
        /// Create and add a new Location to Repo
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        public Location AddLocation(string name, string address, Guid managerId);
        public List<Order> GetOrdersByCustomerId(Guid CustomerID);
        public Order GetOrder(int OrderID);
        /// <summary>
        /// Place an order for a customer/locaiton combination
        /// Also sends a confirmation email to users email addr
        /// </summary>
        /// <param name="location"></param>
        /// <param name="customer"></param>
        /// <param name="items"></param>
        public void PlaceOrder(Order order);
        public void updateItemInStock(InventoryItem item);
        public List<InventoryItem> getInventory(int LocationId);
        public List<Order> GetOrdersByLocationId(int LocationID);

    }
}