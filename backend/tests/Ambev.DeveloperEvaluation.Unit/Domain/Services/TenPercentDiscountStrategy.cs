using Ambev.DeveloperEvaluation.Domain.Services.DiscountStrategies;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Services;

public class TenPercentDiscountStrategyTests
{
    private readonly TenPercentDiscountStrategy _strategy;

    public TenPercentDiscountStrategyTests()
    {
        _strategy = new TenPercentDiscountStrategy();
    }

    [Theory]
    [InlineData(3, false)]
    [InlineData(4, true)]
    [InlineData(9, true)]
    [InlineData(10, false)]
    public void IsApplicable_ShouldReturnCorrectResult(int quantity, bool expected)
    {
        // Act
        var result = _strategy.IsApplicable(quantity);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(4, 10, 4)]  // 4 * 10 * 0.1 = 4
    [InlineData(5, 20, 10)] // 5 * 20 * 0.1 = 10
    [InlineData(9, 15, 13.5)] // 9 * 15 * 0.1 = 13.5
    public void CalculateDiscount_ShouldReturnCorrectDiscount(int quantity, decimal unitPrice, decimal expected)
    {
        // Act
        var result = _strategy.CalculateDiscount(quantity, unitPrice);

        // Assert
        Assert.Equal(expected, result);
    }
}