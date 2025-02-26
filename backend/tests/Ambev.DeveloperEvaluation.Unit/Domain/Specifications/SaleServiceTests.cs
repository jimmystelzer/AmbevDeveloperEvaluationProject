using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Services;

public class SaleServiceTests
{
    private readonly ISaleDiscountService _discountService;
    private readonly ISaleRepository _saleRepository;
    private readonly IEventService _eventService;
    private readonly SaleService _saleService;

    public SaleServiceTests()
    {
        _discountService = Substitute.For<ISaleDiscountService>();
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventService = Substitute.For<IEventService>();
        _saleService = new SaleService(_discountService, _saleRepository, _eventService);
    }

    [Fact]
    public void CreateSaleItem_ShouldReturnValidSaleItem_WhenQuantityIsValid()
    {
        // Arrange
        _discountService.IsValidQuantity(Arg.Any<int>()).Returns(true);
        
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var productName = "Test Product";
        var quantity = 5;
        var unitPrice = 10.0m;
        
        // Act
        var result = _saleService.CreateSaleItem(saleId, productId, productName, quantity, unitPrice);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(saleId, result.SaleId);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal(productName, result.ProductName);
        Assert.Equal(quantity, result.Quantity);
        Assert.Equal(unitPrice, result.UnitPrice);
    }

    [Fact]
    public void CreateSaleItem_ShouldThrowException_WhenQuantityIsInvalid()
    {
        // Arrange
        _discountService.IsValidQuantity(Arg.Any<int>()).Returns(false);
        
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var productName = "Test Product";
        var quantity = 21; // Invalid quantity
        var unitPrice = 10.0m;
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            _saleService.CreateSaleItem(saleId, productId, productName, quantity, unitPrice));
    }

    [Fact]
    public void ApplyDiscounts_ShouldApplyCorrectDiscounts_ToAllItems()
    {
        // Arrange
        var sale = SaleTestData.GenerateSaleWithItems(3);
        
        _discountService.CalculateDiscount(Arg.Any<int>(), Arg.Any<decimal>())
            .Returns(10.0m); // Mock a fixed discount
            
        // Act
        _saleService.ApplyDiscounts(sale);
        
        // Assert
        foreach (var item in sale.Items)
        {
            Assert.Equal(10.0m, item.Discount);
        }
        
        // Verify discount service was called for each item
        _discountService.Received(3).CalculateDiscount(Arg.Any<int>(), Arg.Any<decimal>());
    }

    [Fact]
    public async Task CreateSaleAsync_ShouldCreateAndPublishEvent()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(SaleTestData.GenerateValidSaleItem());
        _saleRepository.CreateAsync(sale).Returns(sale);
        
        // Act
        var result = await _saleService.CreateSaleAsync(sale);
        
        // Assert
        Assert.Equal(sale, result);
        await _saleRepository.Received(1).CreateAsync(sale);
        await _eventService.Received(1).PublishAsync(Arg.Any<SaleCreatedEvent>());
    }

    [Fact]
    public async Task DeleteSaleAsync_ShouldCancelSaleAndPublishEvent()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var saleId = sale.Id;
        
        _saleRepository.GetByIdAsync(saleId).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>()).Returns(sale);
        
        // Act
        var result = await _saleService.DeleteSaleAsync(saleId);
        
        // Assert
        Assert.True(result);
        await _saleRepository.Received(1).GetByIdAsync(saleId);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>());
        await _eventService.Received(1).PublishAsync(Arg.Any<SaleCancelledEvent>());
    }

    [Fact]
    public async Task DeleteSaleAsync_ShouldReturnFalse_WhenSaleNotFound()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _saleRepository.GetByIdAsync(saleId).Returns((Sale?)null);
        
        // Act
        var result = await _saleService.DeleteSaleAsync(saleId);
        
        // Assert
        Assert.False(result);
        await _saleRepository.Received(1).GetByIdAsync(saleId);
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>());
        await _eventService.DidNotReceive().PublishAsync(Arg.Any<SaleCancelledEvent>());
    }

    [Fact]
    public async Task DeleteSaleAsync_ShouldThrowException_WhenSaleAlreadyCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateCancelledSale();
        var saleId = sale.Id;
        
        _saleRepository.GetByIdAsync(saleId).Returns(sale);
        
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _saleService.DeleteSaleAsync(saleId));
        
        await _saleRepository.Received(1).GetByIdAsync(saleId);
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>());
        await _eventService.DidNotReceive().PublishAsync(Arg.Any<SaleCancelledEvent>());
    }
}