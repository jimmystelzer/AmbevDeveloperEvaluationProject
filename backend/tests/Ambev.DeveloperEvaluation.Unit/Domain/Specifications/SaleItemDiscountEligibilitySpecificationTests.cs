using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications;

public class SaleItemDiscountEligibilitySpecificationTests
{
    private readonly SaleItemDiscountEligibilitySpecification _specification;

    public SaleItemDiscountEligibilitySpecificationTests()
    {
        _specification = new SaleItemDiscountEligibilitySpecification();
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(3, false)]
    [InlineData(4, true)]
    [InlineData(10, true)]
    [InlineData(20, true)]
    public void IsSatisfiedBy_ShouldReturnCorrectResult(int quantity, bool expected)
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItemWithQuantity(quantity);
        
        // Act
        var result = _specification.IsSatisfiedBy(item);
        
        // Assert
        Assert.Equal(expected, result);
    }
}