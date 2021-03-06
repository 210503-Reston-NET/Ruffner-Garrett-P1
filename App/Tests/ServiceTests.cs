using Microsoft.EntityFrameworkCore;
using StoreModels;
using Data;
using Xunit;
using System.Collections.Generic;
using Moq;
using System;
using Service;
using Serilog;

namespace Tests
{
    public class ServiceTests
    {
        private readonly DbContextOptions<StoreDBContext> options;
        public ServiceTests(){
            options = new DbContextOptionsBuilder<StoreDBContext>().UseSqlite("Filename=OtherTest.db").Options;

            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        } 
        [Fact]
        public void AddToInventoryTest(){
            //Create Location

            Location c = new Location(){LocationName = "name", Address="address"};
            using (var context = new StoreDBContext(options)){
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                IServices service = new Services(new RepoDB(context));
                
                service.AddLocation("name", "alksdjf", new Guid());
                service.AddProduct("my product", 10);               
            }
            
            //Create Product
            Product p = new Product("My Product", 12.99);
            using (var context = new StoreDBContext(options)){
            IServices service = new Services(new RepoDB(context));
               var products =  service.GetAllProducts();
               var locations = service.GetAllLocations();
               Assert.NotEmpty(locations);
               Assert.True(locations.Count ==1);
               Assert.True(1==products.Count);
               //get location and product ID
               int pid = products[0].ProductID;
               int lid = locations[0].LocationID;

               service.AddProductToInventory(lid,pid, 0);
               
            }

            using (var context = new StoreDBContext(options)){
                IServices service = new Services(new RepoDB(context)); 
                var locations = service.GetAllLocations();
                Assert.True(1 == locations[0].InventoryItems.Count);
            }
        }


        [Fact]
        public void OrdersTest(){
            //Need a Location and a product for the location
            Location c = new Location(){LocationName = "name", Address="address"};
            using (var context = new StoreDBContext(options)){
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                IServices service = new Services(new RepoDB(context));
                
                service.AddLocation("name", "alksdjf", new Guid());
                service.AddProduct("my product", 10);  
                var products =  service.GetAllProducts();
                var locations = service.GetAllLocations();
                Assert.NotEmpty(locations);
                Assert.True(locations.Count ==1);
                Assert.True(1==products.Count);
                //get location and product ID
                int pid = products[0].ProductID;
                int lid = locations[0].LocationID;

                service.AddProductToInventory(lid,pid, 1);             
            }
            
            //Add order            
            using (var context = new StoreDBContext(options)){
            IServices service = new Services(new RepoDB(context));
                var products =  service.GetAllProducts();
                var locations = service.GetAllLocations();
                Assert.NotEmpty(locations);
                Assert.True(locations.Count ==1);
                Assert.True(1==products.Count);
                //get location and product ID
                int lid = locations[0].LocationID;
                int pid = products[0].ProductID;
                var productsl = service.GetAllProducts();

                var oi = new List<OrderItem>();
                oi.Add(new OrderItem(){ProductID = pid,  Quantity = 1, Product = productsl[0]});
                Order order = new Order(){
                    LocationID = lid,
                    OrderItems = oi,
                    CustomerID = new Guid(),
                    Total = 0
                };
                Assert.Throws<Exception>(() => service.PlaceOrder(order));

            }
        }


        [Fact]
        public void GetReviewsShouldReturnAverage()
        {
            //Arrange
            // Create a stub of the IRepository

            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetAllCustomers()).Returns
                (
                    new List<ApplicationUser>()
                    {
                        new ApplicationUser(){Name = "Bill", Address = "1829 Street", UserName="email@email.com"}
                    }

                );
            //Build a Service object
            var serviceBL = new Services(mockRepo.Object);

            //Act
            var result = serviceBL.GetAllCustomers();

            //Assert
            // Asserting that given the test input, the average should be five
            Assert.Equal("Bill", result[0].Name);
        }

        [Fact]
        public void OrderTotalTest(){
            var mockRepo = new Mock<IRepository>();
            mockRepo.Setup(x => x.GetOrderByID(1)).Returns
                (
                    new Order()
                    {
                        OrderID = 1,
                        LocationID = 1,
                        OrderItems = new List<OrderItem>()
                        {
                            new OrderItem(){
                                ProductID = 1,
                                Product = new Product(){
                                    ProductID = 1,
                                    Name = "thing",
                                    Price = 3
                                },
                                Quantity = 4
                            },
                            new OrderItem(){
                                ProductID = 2,
                                Product = new Product(){
                                    ProductID = 2,
                                    Name = "otherthing",
                                    Price = 6
                                },
                                Quantity = 2
                            }
                        },
                        Total = 0
                    }

                );
            var serviceBL = new Services(mockRepo.Object);    
            var result = serviceBL.GetOrder(1);
            Assert.Equal(1, result.LocationID);
            Assert.Equal(24, result.Total);

        }
        [Fact]
        public void AddDuplicateItemToInventoryTest(){
            //Create Location

            Location c = new Location(){LocationName = "name", Address="address"};
            using (var context = new StoreDBContext(options)){
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                IServices service = new Services(new RepoDB(context));
                
                service.AddLocation("name", "alksdjf", new Guid());
                service.AddProduct("my product", 10);               
            }
            
            //Create Product
            Product p = new Product("My Product", 12.99);
            using (var context = new StoreDBContext(options)){
            IServices service = new Services(new RepoDB(context));
               var products =  service.GetAllProducts();
               var locations = service.GetAllLocations();
               Assert.NotEmpty(locations);
               Assert.True(locations.Count ==1);
               Assert.True(1==products.Count);
               //get location and product ID
               int pid = products[0].ProductID;
               int lid = locations[0].LocationID;

               service.AddProductToInventory(lid,pid, 0);
               Assert.Throws<Exception>(() => service.AddProductToInventory(lid,pid, 0));
               
            }

            using (var context = new StoreDBContext(options)){
                IServices service = new Services(new RepoDB(context)); 
                var locations = service.GetAllLocations();
                Assert.True(1 == locations[0].InventoryItems.Count);
            }
        }

        [Fact]
        public void UpdateStockTest(){
            
            Location c = new Location(){LocationName = "name", Address="address"};
            using (var context = new StoreDBContext(options)){
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                IServices service = new Services(new RepoDB(context));
                
                service.AddLocation("name", "alksdjf", new Guid());
                service.AddProduct("my product", 10);               
            }

            // add to inventory
            using (var context = new StoreDBContext(options)){
            IServices service = new Services(new RepoDB(context));
               var products =  service.GetAllProducts();
               var locations = service.GetAllLocations();
               Assert.NotEmpty(locations);
               Assert.True(locations.Count ==1);
               Assert.True(1==products.Count);
               //get location and product ID
               int pid = products[0].ProductID;
               int lid = locations[0].LocationID;

               service.AddProductToInventory(lid,pid, 3);
               
            }

            //update inventory
            using (var context = new StoreDBContext(options)){
                IServices service = new Services(new RepoDB(context)); 
                var locations = service.GetAllLocations();
                var ii = locations[0].InventoryItems[0];
                int before = ii.Quantity;
                ii.Quantity = before +4;
                service.updateItemInStock(ii);
            }

            //ensure updated
            using (var context = new StoreDBContext(options)){
                IServices service = new Services(new RepoDB(context)); 
                var locations = service.GetAllLocations();
                var ii = locations[0].InventoryItems[0];
                Assert.True(7 == ii.Quantity);
            }
        }

        [Fact]
        //Tests the retrival of all products as well as adding a new product
        public void TestGetOrderById()
        {
            var mockRepo = new Mock<IRepository>();
            var order = new Order()
                    {
                        CustomerID = new ApplicationUser().Id,
                        OrderID = 1,
                        LocationID = 1,
                        OrderItems = new List<OrderItem>()
                        {
                            new OrderItem(){
                                ProductID = 1,
                                Product = new Product(){
                                    ProductID = 1,
                                    Name = "thing",
                                    Price = 3
                                },
                                Quantity = 4
                            },
                            new OrderItem(){
                                ProductID = 2,
                                Product = new Product(){
                                    ProductID = 2,
                                    Name = "otherthing",
                                    Price = 6
                                },
                                Quantity = 2
                            }
                        },
                        Total = 0
                    };
            var location = new Location(){
                LocationID = 1,
                LocationName = "slad",
                Address = "flkdf"
            };

            mockRepo.Setup(x => x.GetOrderByID(1)).Returns(
                order
            );
            var serviceBL = new Services(mockRepo.Object);    
            var result = serviceBL.GetOrder(1);

            Assert.NotNull(serviceBL.GetOrder(1));  
    
        }


        [Fact]
        public void itemgreaterthanzero(){
            
            var order = new InventoryItem()
                {                        
                    ProductID = 1,
                    Product = new Product(){
                        ProductID = 1,
                        Name = "thing",
                        Price = 3
                    },
                    Quantity = 5                        
                };
           

           
            Assert.Throws<Exception>(()=> order.Quantity = -3);

            
        }
    }
}
