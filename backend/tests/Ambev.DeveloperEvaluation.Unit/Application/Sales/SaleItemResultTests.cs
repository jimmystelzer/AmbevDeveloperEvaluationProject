using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class SaleItemResultTests
    {
        [Fact]
        public void SaleItemResult_Should_Set_Properties_Correctly()
        {
            // Arrange
            var saleItemResult = new SaleItemResult
            {
                Id = Guid.NewGuid(),
                ProductName = "Test Product",
                Quantity = 2,
                UnitPrice = 10.00m,
                Discount = 2.00m,
                TotalAmount = 18.00m
            };

            // Assert
            Assert.NotEqual(Guid.Empty, saleItemResult.Id);
            Assert.Equal("Test Product", saleItemResult.ProductName);
            Assert.Equal(2, saleItemResult.Quantity);
            Assert.Equal(10.00m, saleItemResult.UnitPrice);
            Assert.Equal(2.00m, saleItemResult.Discount);
            Assert.Equal(18.00m, saleItemResult.TotalAmount);
        }

        [Fact]
        public void SaleItemResult_Can_Be_Created_And_Properties_Read()
        {
            // Arrange
            var id = Guid.NewGuid();
            var productName = "Sample Product";
            var quantity = 3;
            var unitPrice = 5.50m;
            var discount = 1.50m;
            var totalAmount = 15.00m;

            // Act
            var saleItemResult = new SaleItemResult
            {
                Id = id,
                ProductName = productName,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Discount = discount,
                TotalAmount = totalAmount
            };

            // Assert
            Assert.Equal(id, saleItemResult.Id);
            Assert.Equal(productName, saleItemResult.ProductName);
            Assert.Equal(quantity, saleItemResult.Quantity);
            Assert.Equal(unitPrice, saleItemResult.UnitPrice);
            Assert.Equal(discount, saleItemResult.Discount);
            Assert.Equal(totalAmount, saleItemResult.TotalAmount);
        }
    }
}