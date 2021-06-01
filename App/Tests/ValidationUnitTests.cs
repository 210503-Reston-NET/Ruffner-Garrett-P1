using System;
using Xunit;
using StoreModels;


namespace Tests
{
    public class ValidationUnitTests
    {
        // Test Models to make sure they only take good data

        [Fact]
        public void PriceValidationTest(){
            Product p = new Product();
            Assert.Throws<Exception>(() =>p.Price = -5.05);
        }
        [Fact]
        public void QuantityValidationTest(){
            InventoryItem ii = new InventoryItem();
            Assert.Throws<Exception>(() =>ii.Quantity = -5);
        }
        [Fact]
        public void QuantityValidationTest2(){
            OrderItem ii = new OrderItem();
            Assert.Throws<Exception>(() =>ii.Quantity = -5);
        }

        [Fact]
        public void ProductNameValidationTest(){
            Product p = new Product();
            Assert.Throws<Exception>(() =>p.Name = "");
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
        [Theory]
        [InlineData (-10)]
        [InlineData (-5)]
        public void ItemQuantityTest(int quantity)
        {
            OrderItem oi = new OrderItem();
            Assert.Throws<Exception>(() =>oi.Quantity = quantity);
            InventoryItem ii = new InventoryItem();
            Assert.Throws<Exception>(() =>ii.Quantity = quantity);
        }


    }
}
        // [Fact]
        // public void CityValidationTest()
        // {
        //     Assert.True(VS.ValidateCityName("St. Louis"));
        //     Assert.False(VS.ValidateCityName("city #4"));
        //     Assert.False(VS.ValidateAddress(" "));
        //     Assert.False(VS.ValidateAddress(""));
        // }
        // [Fact]
        // public void NameValidationTest()
        // {
        //     Assert.True(VS.ValidatePersonName("Daniel McDowell"));
        //     Assert.False(VS.ValidatePersonName("sk8r boi"));
        //     Assert.False(VS.ValidatePersonName(" "));
        //     Assert.False(VS.ValidatePersonName(""));
        // }
        // [Fact]
        // public void AddressValidationTest()
        // {
        //     Assert.True(VS.ValidateAddress("8323 Somewhere Drive"));
        //     Assert.True(VS.ValidateAddress("2938 Apt. #289, place, city 20399"));
        //     Assert.False(VS.ValidateAddress("asfdVDKL3893e(*#@*"));
        //     Assert.False(VS.ValidateAddress(" "));
        //     Assert.False(VS.ValidateAddress(""));
        // }
        // [Fact]
        // public void DoubleValidatioTest()
        // {
        //     Assert.True(VS.ValidateDouble("39.40"));
        //     Assert.True(VS.ValidateDouble("-23.34"));
        //     Assert.True(VS.ValidateDouble("-8"));
        //     Assert.True(VS.ValidateDouble("70"));
        //     Assert.False(VS.ValidateDouble(" "));
        //     Assert.False(VS.ValidateDouble(""));
        // }
        // [Fact]
        // public void StringValidationTest()
        // {
        //     Assert.True(VS.ValidateString("lskdfjl238 ei328*(#@* i2398"));
        //     Assert.False(VS.ValidateString(" "));
        //     Assert.False(VS.ValidateString(""));

        // }
        // [Fact]
        // public void ValidtateAddressTest()
        // {
        //     Assert.True(VS.ValidateAddress("LKsdfjeie#Apt93392,-,     stuff29"));
        //     Assert.False(VS.ValidateAddress("!@$%&*(){}[]\\_=|+?"));
        //     Assert.False(VS.ValidateAddress(" "));
        //     Assert.False(VS.ValidateAddress(""));
        // }
        // [Fact]
        // public void ValidateIntTest()
        // {
        //     Assert.True(VS.ValidateInt("189239829813"));
        //     Assert.True(VS.ValidateInt("-189239829813"));
        //     Assert.True(VS.ValidateInt("-0000189239829813"));
        //     Assert.True(VS.ValidateInt("+00189239829813"));
        //     Assert.False(VS.ValidateInt("+189*239829813"));
        //     Assert.False(VS.ValidateInt("^129"));
        //     Assert.False(VS.ValidateInt("+"));
        //     Assert.False(VS.ValidateInt("-"));
        //     Assert.False(VS.ValidateInt(" "));
        //     Assert.False(VS.ValidateInt(""));
        // }
        // [Fact]
        // public void ValidatPositiveIntTest()
        // {
        //     Assert.True(VS.ValidatePositiveInt("189239829813"));
        //     Assert.True(VS.ValidatePositiveInt("+00189239829813"));
        //     Assert.False(VS.ValidatePositiveInt("+"));
        //     Assert.False(VS.ValidatePositiveInt("-"));
        //     Assert.False(VS.ValidatePositiveInt("+189*239829813"));
        //     Assert.False(VS.ValidatePositiveInt("^129"));
        //     Assert.False(VS.ValidatePositiveInt("-189239829813"));
        //     Assert.False(VS.ValidatePositiveInt("-0000189239829813"));
        //     Assert.False(VS.ValidatePositiveInt(" "));
        //     Assert.False(VS.ValidatePositiveInt(""));
        // }
        // [Fact]
        // public void ValidateNegativeIntTest()
        // {
        //     Assert.True(VS.ValidateNegativeInt("-189239829813"));
        //     Assert.True(VS.ValidateNegativeInt("-00189239829813"));
        //     Assert.False(VS.ValidateNegativeInt("+"));
        //     Assert.False(VS.ValidateNegativeInt("-"));
        //     Assert.False(VS.ValidateNegativeInt("+189*239829813"));
        //     Assert.False(VS.ValidateNegativeInt("^129"));
        //     Assert.False(VS.ValidateNegativeInt("+189239829813"));
        //     Assert.False(VS.ValidateNegativeInt("0000189239829813"));
        //     Assert.False(VS.ValidateNegativeInt(" "));
        //     Assert.False(VS.ValidateNegativeInt(""));
        // }
        // [Fact]
        // public void ValidateIntWithinRangeTest()
        // {
        //     Assert.True(VS.ValidateIntWithinRange("01", 0,10));
        //     Assert.True(VS.ValidateIntWithinRange("1", 0,1));
        //     Assert.True(VS.ValidateIntWithinRange("0", 0,1));
        //     Assert.True(VS.ValidateIntWithinRange("-10", -11,1));
        //     Assert.False(VS.ValidateIntWithinRange("-10", 0,10));
        //     Assert.False(VS.ValidateIntWithinRange("", 0,1));
        //     Assert.False(VS.ValidateIntWithinRange(" ", 0,1));
        // }
        // [Fact]
        // public void ValidateEmailTest()
        // {
        //     Assert.True(VS.ValidateEmail("asdf@asdf.asdf"));
        //     Assert.True(VS.ValidateEmail("101@a.b"));
        //     Assert.False(VS.ValidateEmail("@lkasjdff.sdkf.sdf"));
        //     Assert.False(VS.ValidateEmail("a@lkasjdf@...83f.sdkf.sdf.0"));
        //     Assert.False(VS.ValidateEmail(" "));
        //     Assert.False(VS.ValidateEmail(""));
        // }
        

