using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications;

public class SaleItemQuantitySpecificationTests
{
    private readonly SaleItemQuantitySpecification _specification;

    public SaleItemQuantitySpecificationTests()
    {
        _specification = new SaleItemQuantitySpecification();
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(5, true)]
    [InlineData(20, true)]
    [InlineData(0, false)]
    [InlineData(21, false)]
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