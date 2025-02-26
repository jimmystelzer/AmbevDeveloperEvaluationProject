using Ambev.DeveloperEvaluation.Domain.Services.DiscountStrategies;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Services;

public class TwentyPercentDiscountStrategyTests
{
    private readonly TwentyPercentDiscountStrategy _strategy;

    public TwentyPercentDiscountStrategyTests()
    {
        _strategy = new TwentyPercentDiscountStrategy();
    }

    [Theory]
    [InlineData(9, false)]
    [InlineData(10, true)]
    [InlineData(15, true)]
    [InlineData(20, true)]
    [InlineData(21, false)]
    public void IsApplicable_ShouldReturnCorrectResult(int quantity, bool expected)
    {
        // Act
        var result = _strategy.IsApplicable(quantity);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(10, 10, 20)]  // 10 * 10 * 0.2 = 20
    [InlineData(15, 20, 60)]  // 15 * 20 * 0.2 = 60
    [InlineData(20, 15, 60)]  // 20 * 15 * 0.2 = 60
    public void CalculateDiscount_ShouldReturnCorrectDiscount(int quantity, decimal unitPrice, decimal expected)
    {
        // Act
        var result = _strategy.CalculateDiscount(quantity, unitPrice);

        // Assert
        Assert.Equal(expected, result);
    }
}