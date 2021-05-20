using System;
using StoreModels;
using Xunit;
namespace Tests
{
    public class InternalTests
    {
        [Theory]
        [InlineData (-10)]
        [InlineData (-5)]
        public void ItemQuantityTest(int quantity)
        {
            // Item itm = new Item(new Product("name",1.00),0);
            //     Assert.Throws<Exception>(() => itm.ChangeQuantity(quantity));
        }

        [Theory]
        [InlineData ("name" ,-1.10)]
        [InlineData ("naem",-0.00001)]
        [InlineData ("",5)]
        [InlineData ("   ",5)]

        public void ProductPriceTest(string name, double price)
        {
            Assert.Throws<Exception>(() => new Product(name, price));
        }
    }
}