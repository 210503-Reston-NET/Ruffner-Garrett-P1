using System;
using System.Collections.Generic;
using StoreModels;
namespace Data
{
    public interface IRepository
    {
        /// <summary>
        /// Add new Location to Repo
        /// </summary>
        /// <param name="location"></param>
        public void AddLocation(Location location);
        /// <summary>
        /// Add new Product to Repo
        /// </summary>
        /// <param name="product"></param>
        public void AddProduct(Product product);
        /// <summary>
        /// Returns List Containing all Customers
        /// </summary>
        /// <returns></returns>
        public List<ApplicationUser> GetAllCustomers();
        /// <summary>
        /// Returns List Containing all Locations
        /// </summary>
        /// <returns></returns>
        public List<Location> GetAllLocations();
        /// <summary>
        /// Returns List Containing all Products
        /// </summary>
        /// <returns></returns>
        public List<Product> GetAllProducts();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        public void PlaceOrder(Order order);
        /// <summary>
        /// Gets Orders by Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="price">If True => Sorted by price; False => Sorted by Date</param>
        /// <param name="asc">True => Ascending Order; False => Descending</param>
        /// <returns></returns>
        public List<Order> GetOrdersByCustomerID(Guid CustomerID);
        public Order GetOrderByID(int OrderID);
                /// <summary>
        /// Gets Orders by Location
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="price">If True => Sorted by price; False => Sorted by Date</param>
        /// <param name="asc">True => Ascending Order; False => Descending</param>
        /// <returns></returns>
        public List<Order> GetOrders(Location location, bool price, bool asc);
        /// <summary>
        /// Updates the Provided item in the Locations inventory
        /// </summary>
        /// <param name="location"></param>
        /// <param name="item"></param>
        void UpdateInventoryItem(InventoryItem item);
        /// <summary>
        /// Adds a product to a locations inventory
        /// </summary>
        /// <param name="location"></param>
        /// <param name="item"></param>
        public void AddProductToInventory(Location location, InventoryItem item);
        /// <summary>
        /// Start A Transaction
        /// </summary>
        public void StartTransaction();
        /// <summary>
        /// End A Transaction
        /// </summary>
        /// <param name="success">True if operation was successful</param>
        public void EndTransaction(bool success);
        public Location GetLocationById(int LocationID);
        public Product GetProductById(int ProductID);
        public List<Order> GetOrdersByLocationID(int LocationID);
    }
}