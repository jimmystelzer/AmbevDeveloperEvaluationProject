using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    [Fact(DisplayName = "SaleItem should calculate total amount correctly")]
    public void Given_SaleItemWithPriceAndQuantity_When_CalculatingTotalAmount_Then_ShouldCalculateCorrectly()
    {
        // Arrange
        decimal unitPrice = 10.0m;
        int quantity = 5;
        var item = SaleTestData.GenerateSaleItemWithValues(unitPrice, quantity);

        // Act & Assert
        decimal expectedTotal = unitPrice * quantity;
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    [Fact(DisplayName = "SaleItem should apply discount correctly")]
    public void Given_SaleItem_When_ApplyingDiscount_Then_ShouldReduceTotalAmount()
    {
        // Arrange
        decimal unitPrice = 10.0m;
        int quantity = 5;
        decimal discountAmount = 10.0m;
        var item = SaleTestData.GenerateSaleItemWithValues(unitPrice, quantity);

        // Act
        item.ApplyDiscount(discountAmount);
        decimal expectedTotal = (unitPrice * quantity) - discountAmount;

        // Assert
        Assert.Equal(discountAmount, item.Discount);
        Assert.Equal(expectedTotal, item.TotalAmount);
    }
}
