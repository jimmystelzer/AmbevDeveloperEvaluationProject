using Ambev.DeveloperEvaluation.Domain.Services.DiscountStrategies;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Services;
public class NoDiscountStrategyTests
{
    private readonly NoDiscountStrategy _strategy;

    public NoDiscountStrategyTests()
    {
        _strategy = new NoDiscountStrategy();
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(3, true)]
    [InlineData(4, false)]
    [InlineData(10, false)]
    public void IsApplicable_ShouldReturnCorrectResult(int quantity, bool expected)
    {
        // Act
        var result = _strategy.IsApplicable(quantity);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, 10, 0)]
    [InlineData(3, 20, 0)]
    public void CalculateDiscount_ShouldReturnZero(int quantity, decimal unitPrice, decimal expected)
    {
        // Act
        var result = _strategy.CalculateDiscount(quantity, unitPrice);

        // Assert
        Assert.Equal(expected, result);
    }
}