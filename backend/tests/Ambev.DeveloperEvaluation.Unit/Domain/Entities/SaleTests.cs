using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Sale status should change to Cancelled when cancelled")]
    public void Given_ActiveSale_When_Cancelled_Then_StatusShouldBeCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        
        // Act
        sale.Cancel();

        // Assert
        Assert.Equal(SaleStatus.Cancelled, sale.Status);
    }

    [Fact(DisplayName = "Trying to cancel already cancelled sale should throw exception")]
    public void Given_CancelledSale_When_Cancelled_Then_ShouldThrowException()
    {
        // Arrange
        var sale = SaleTestData.GenerateCancelledSale();
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sale.Cancel());
    }

    [Fact(DisplayName = "Total amount should reflect sum of all item totals")]
    public void Given_SaleWithMultipleItems_When_CalculatingTotalAmount_Then_ShouldEqualSumOfItemTotals()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var items = SaleTestData.GenerateMultipleSaleItems(3);
        decimal expectedTotal = 0;

        // Act
        foreach (var originalItem in items)
        {
            var item = new SaleItem(
                Guid.NewGuid(),
                sale.Id,
                originalItem.ProductId,
                originalItem.ProductName,
                originalItem.Quantity,
                originalItem.UnitPrice
            );
            sale.AddItem(item);
            expectedTotal += item.TotalAmount;
        }

        // Assert
        Assert.Equal(expectedTotal, sale.TotalAmount);
    }
}