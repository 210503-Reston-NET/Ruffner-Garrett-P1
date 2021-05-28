using System;
using System.Collections.Generic;
using StoreModels;
namespace Service
{
    public interface IServices
    {
        // /// <summary>
        // /// Create a new Customer and add it to the repository
        // /// </summary>
        // /// <param name="name"></param>
        // /// <param name="address"></param>
        // /// <param name="email"></param>
        // public void AddCustomer(string name, string address, string email);
        /// <summary>
        /// Get Customers From Repo
        /// </summary>
        /// <returns></returns>
        public List<ApplicationUser> GetAllCustomers();
        /// <summary>
        /// Get Locations From Repo
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Search for a customer by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ApplicationUser SearchCustomers(string name);
        public List<Order> GetOrdersByCustomerId(Guid CustomerID);
        public List<Order> GetOrders(Location location, bool price, bool asc);
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

    }
}