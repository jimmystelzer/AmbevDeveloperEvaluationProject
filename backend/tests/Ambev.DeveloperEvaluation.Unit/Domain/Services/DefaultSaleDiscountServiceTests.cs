using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Services.DiscountStrategies;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Services;

public class DefaultSaleDiscountServiceTests
{
    private readonly DefaultSaleDiscountService _service;

    public DefaultSaleDiscountServiceTests()
    {
        var strategies = new List<IDiscountStrategy>
        {
            new NoDiscountStrategy(),
            new TenPercentDiscountStrategy(),
            new TwentyPercentDiscountStrategy()
        };
        _service = new DefaultSaleDiscountService(strategies);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(20, true)]
    [InlineData(21, false)]
    public void IsValidQuantity_ShouldReturnCorrectResult(int quantity, bool expected)
    {
        // Act
        var result = _service.IsValidQuantity(quantity);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(3, false)]
    [InlineData(4, true)]
    [InlineData(10, true)]
    [InlineData(20, true)]
    public void CanApplyDiscount_ShouldReturnCorrectResult(int quantity, bool expected)
    {
        // Act
        var result = _service.CanApplyDiscount(quantity);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, 10, 0)]     // No discount for < 4
    [InlineData(3, 20, 0)]     // No discount for < 4
    [InlineData(4, 10, 4)]     // 4 * 10 * 0.1 = 4
    [InlineData(9, 20, 18)]    // 9 * 20 * 0.1 = 18
    [InlineData(10, 10, 20)]   // 10 * 10 * 0.2 = 20
    [InlineData(20, 15, 60)]   // 20 * 15 * 0.2 = 60
    [InlineData(21, 10, 0)]    // Invalid quantity, no discount
    public void CalculateDiscount_ShouldReturnCorrectDiscount(int quantity, decimal unitPrice, decimal expected)
    {
        // Act
        var result = _service.CalculateDiscount(quantity, unitPrice);

        // Assert
        Assert.Equal(expected, result);
    }
}
