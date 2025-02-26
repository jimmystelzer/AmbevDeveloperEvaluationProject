// SaleRepositoryTests.cs
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Repositories;

public class SaleRepositoryTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly SaleRepository _repository;

    public SaleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"SaleDb_{Guid.NewGuid()}")
            .Options;

        _context = new DefaultContext(options);
        _repository = new SaleRepository(_context);
    }

    [Fact(DisplayName = "CreateAsync should add sale to database")]
    public async Task CreateAsync_ShouldAddSaleToDatabase()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var result = await _repository.CreateAsync(sale);

        // Assert
        var savedSale = await _context.Sales.FindAsync(result.Id);
        Assert.NotNull(savedSale);
        Assert.Equal(sale.SaleNumber, savedSale.SaleNumber);
    }

    [Fact(DisplayName = "GetByIdAsync should retrieve sale with items")]
    public async Task GetByIdAsync_ShouldRetrieveSaleWithItems()
    {
        // Arrange
        var sale = SaleTestData.GenerateSaleWithItems(3);
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(sale.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sale.Id, result.Id);
        Assert.Equal(3, result.Items.Count);
    }

    [Fact(DisplayName = "UpdateAsync should update sale in database")]
    public async Task UpdateAsync_ShouldUpdateSaleInDatabase()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        // Update sale
        sale.Cancel();

        // Act
        var result = await _repository.UpdateAsync(sale);

        // Assert
        var updatedSale = await _context.Sales.FindAsync(sale.Id);
        Assert.Equal(SaleStatus.Cancelled, updatedSale.Status);
    }

    [Fact(DisplayName = "DeleteAsync should remove sale from database")]
    public async Task DeleteAsync_ShouldRemoveSaleFromDatabase()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(sale.Id);

        // Assert
        Assert.True(result);
        var deletedSale = await _context.Sales.FindAsync(sale.Id);
        Assert.Null(deletedSale);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}