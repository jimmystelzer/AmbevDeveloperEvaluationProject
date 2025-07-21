using Xunit;
using NSubstitute;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class GetSaleItemResultTests
    {
        [Fact]
        public void GetSaleItemResult_Properties_Should_Set_And_Get()
        {
            // Arrange
            var item = new GetSaleItemResult
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                ProductName = "Test Product",
                Quantity = 2,
                UnitPrice = 10.50m,
                Discount = 1.00m,
                TotalAmount = 20.00m
            };

            // Assert
            Assert.NotNull(item);
            Assert.NotEqual(Guid.Empty, item.Id);
            Assert.NotEqual(Guid.Empty, item.ProductId);
            Assert.Equal("Test Product", item.ProductName);
            Assert.Equal(2, item.Quantity);
            Assert.Equal(10.50m, item.UnitPrice);
            Assert.Equal(1.00m, item.Discount);
            Assert.Equal(20.00m, item.TotalAmount);
        }
    }
}