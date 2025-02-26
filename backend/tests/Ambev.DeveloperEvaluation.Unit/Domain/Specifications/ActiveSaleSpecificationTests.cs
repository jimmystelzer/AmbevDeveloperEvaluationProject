using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications;

public class ActiveSaleSpecificationTests
{
    private readonly ActiveSaleSpecification _specification;

    public ActiveSaleSpecificationTests()
    {
        _specification = new ActiveSaleSpecification();
    }

    [Fact]
    public void IsSatisfiedBy_ShouldReturnTrue_ForActiveSale()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        
        // Act
        var result = _specification.IsSatisfiedBy(sale);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_ShouldReturnFalse_ForCancelledSale()
    {
        // Arrange
        var sale = SaleTestData.GenerateCancelledSale();
        
        // Act
        var result = _specification.IsSatisfiedBy(sale);
        
        // Assert
        Assert.False(result);
    }
}