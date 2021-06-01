using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StoreModels;
using Data;
using Xunit;
using System.Collections.Generic;
using Moq;
using System;
using Service;

namespace Tests
{
    public class DBUnitTests
    {
        private readonly DbContextOptions<StoreDBContext> options;
        public DBUnitTests(){
            options = new DbContextOptionsBuilder<StoreDBContext>().UseSqlite("Filename=Test.db").Options;
        }

        [Fact]
        //Tests the retrival of all locations as well as adding a new location
        public void TestLocationBeingAdded()
        {
           Location c = new Location(){LocationName = "name", Address="address"};
            using (var context = new StoreDBContext(options)){
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                IRepository _repo = new RepoDB(context);
                _repo.AddLocation(c);
           
            }
            //Using a new context check for Location
            using (var context = new StoreDBContext(options)){
                IRepository _repo = new RepoDB(context);
                List<Location> cs = _repo.GetAllLocations();
                Location cdb = cs.ToArray()[0];
                Assert.Equal(c.LocationID, cdb.LocationID);
                Assert.Equal(c.LocationName, cdb.LocationName);
                Assert.Equal(c.Address, cdb.Address);
                Assert.True(cs.Count == 1);
                context.Database.EnsureDeleted();

           }
        }
        [Fact]
        //Tests the retrival of all products as well as adding a new product
        public void TestProductBeingAdded()
        {
            // var mockRepo = new Mock<IRepository>();
            Product c = new Product("My Product", 12.99);
            using (var context = new StoreDBContext(options)){
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                IRepository _repo = new RepoDB(context);
                _repo.AddProduct(c);
                
           
            }
            //Using a new context check for Product
            using (var context = new StoreDBContext(options)){
                IRepository _repo = new RepoDB(context);
                List<Product> cs = _repo.GetAllProducts();
                Product cdb = cs.ToArray()[0];
                Assert.Equal(c.Name, cdb.Name);
                Assert.Equal(c.Price, cdb.Price);
                Assert.True(cs.Count == 1);
                Assert.Equal(c.ProductID, cdb.ProductID);
                context.Database.EnsureDeleted();

           }
        }
        
    }
}