using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Models = StoreModels;
using Entity = Data.Entities;
using Xunit;
using Data;
using System.Collections.Generic;

namespace Tests
{
    public class DBUnitTests
    {
        private readonly DbContextOptions<Entity.p0Context> options;
        public DBUnitTests(){
            options = new DbContextOptionsBuilder<Entity.p0Context>().UseSqlite("Filename=Test.db").Options;
        }
        [Fact]
        //Tests the retrival of all customers as well as adding a new customer
        public void TestCustomersBeingAdded()
        {
            Models.Customer c = new Models.Customer("Billy Joe", "123 Street", new MailAddress("asdf@somewher.net"));
            using (var context = new Entity.p0Context(options)){
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                IRepository _repo = new RepoDB(context);
                _repo.AddCustomer(c);
                context.SaveChanges();
           
            }
            //Using a new context check for customer
            using (var context = new Entity.p0Context(options)){
                IRepository _repo = new RepoDB(context);
                List<Models.Customer> cs = _repo.GetAllCustomers();
                Models.Customer cdb = cs.ToArray()[0];
                Assert.True(cs.Count == 1);
                Assert.Equal(c.Name, cdb.Name);
                Assert.Equal(c.Address, cdb.Address);
                Assert.Equal(c.Email, cdb.Email);
                context.Database.EnsureDeleted();

           }
        }
        [Fact]
        //Tests the retrival of all locations as well as adding a new location
        public void TestLocationBeingAdded()
        {
            Models.Location c = new Models.Location("My Location", "123 Peanut Street, Nowhere, TX 39849");
            using (var context = new Entity.p0Context(options)){
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                IRepository _repo = new RepoDB(context);
                _repo.AddLocation(c);
                context.SaveChanges();
           
            }
            //Using a new context check for Location
            using (var context = new Entity.p0Context(options)){
                IRepository _repo = new RepoDB(context);
                List<Models.Location> cs = _repo.GetAllLocations();
                Models.Location cdb = cs.ToArray()[0];
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
            Models.Product c = new Models.Product("My Product", 12.99);
            using (var context = new Entity.p0Context(options)){
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                IRepository _repo = new RepoDB(context);
                _repo.AddProduct(c);
                context.SaveChanges();
           
            }
            //Using a new context check for Product
            using (var context = new Entity.p0Context(options)){
                IRepository _repo = new RepoDB(context);
                List<Models.Product> cs = _repo.GetAllProducts();
                Models.Product cdb = cs.ToArray()[0];
                Assert.Equal(c.ProductName, cdb.ProductName);
                Assert.Equal(c.Price, cdb.Price);
                Assert.True(cs.Count == 1);
                context.Database.EnsureDeleted();

           }

        }
        
    }
}