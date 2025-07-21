using Xunit;
using NSubstitute;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using System;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class UpdateSaleItemResultTests
    {
        [Fact]
        public void UpdateSaleItemResult_ValidData_ShouldCreateInstance()
        {
            // Arrange
            var id = Guid.NewGuid();
            var productName = "Test Product";
            var quantity = 2;
            var unitPrice = 10.00m;
            var discount = 0.10m;
            var totalAmount = 18.00m;

            // Act
            var result = new UpdateSaleItemResult()
            {
                Id = id,
                ProductName = productName,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Discount = discount,
                TotalAmount = totalAmount
            };

            // Assert
            Assert.Equal(id, result.Id);
            Assert.Equal(productName, result.ProductName);
            Assert.Equal(quantity, result.Quantity);
            Assert.Equal(unitPrice, result.UnitPrice);
            Assert.Equal(discount, result.Discount);
            Assert.Equal(totalAmount, result.TotalAmount);
        }
    }
}