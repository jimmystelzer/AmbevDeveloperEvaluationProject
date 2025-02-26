using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications;

public class ValidSaleSpecificationTests
{
    private readonly ValidSaleSpecification _specification;

    public ValidSaleSpecificationTests()
    {
        _specification = new ValidSaleSpecification();
    }

    [Fact]
    public void IsSatisfiedBy_ShouldReturnTrue_ForValidSale()
    {
        // Arrange
        var sale = SaleTestData.GenerateSaleWithItems(1);
        
        // Act
        var result = _specification.IsSatisfiedBy(sale);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_ShouldReturnFalse_ForSaleWithoutItems()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        // No items added
        
        // Act
        var result = _specification.IsSatisfiedBy(sale);
        
        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void IsSatisfiedBy_ShouldReturnFalse_ForSaleWithInvalidFields(string? invalidValue)
    {
        // Arrange
        var sale = SaleTestData.GenerateSaleWithItems(1);
        
        // Use reflection to change private properties for testing
        var prop = typeof(Sale).GetProperty("SaleNumber");
        if (prop != null)
        {
            prop.SetValue(sale, invalidValue);
        }
        
        // Act
        var result = _specification.IsSatisfiedBy(sale);
        
        // Assert
        Assert.False(result);
    }
}