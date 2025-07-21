// /home/jimmy/Downloads/Ambev/backend/tests/Ambev.DeveloperEvaluation.Unit/Repositories/SaleRepositoryTests.cs
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Unit.Repositories;

public class SaleRepositoryTests
{
    private readonly DbContextOptions<DefaultContext> _options;

    public SaleRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: "SaleRepositoryTests")
            .Options;
    }

    [Fact]
    public async Task CreateAsync_ValidSale_ReturnsCreatedSale()
    {
        using (var context = new DefaultContext(_options))
        {
            var repository = new SaleRepository(context);
            var sale = new Sale(
                Guid.NewGuid(),
                "SN123",
                DateTime.Now,
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch"
            );

            var createdSale = await repository.CreateAsync(sale);

            Assert.NotNull(createdSale);
            Assert.Equal(sale.SaleNumber, createdSale.SaleNumber);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsSale()
    {
        using (var context = new DefaultContext(_options))
        {
            var repository = new SaleRepository(context);
            var sale = new Sale(
                Guid.NewGuid(),
                "SN123",
                DateTime.Now,
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch"
            );

            context.Sales.Add(sale);
            await context.SaveChangesAsync();

            var retrievedSale = await repository.GetByIdAsync(sale.Id);

            Assert.NotNull(retrievedSale);
            Assert.Equal(sale.Id, retrievedSale.Id);
        }
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        using (var context = new DefaultContext(_options))
        {
            var repository = new SaleRepository(context);
            var retrievedSale = await repository.GetByIdAsync(Guid.NewGuid());

            Assert.Null(retrievedSale);
        }
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_ReturnsTrue()
    {
        using (var context = new DefaultContext(_options))
        {
            var repository = new SaleRepository(context);
            var sale = new Sale(
                Guid.NewGuid(),
                "SN123",
                DateTime.Now,
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch"
            );

            context.Sales.Add(sale);
            await context.SaveChangesAsync();

            var result = await repository.DeleteAsync(sale.Id);

            Assert.True(result);
            Assert.Null(await repository.GetByIdAsync(sale.Id));
        }
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ReturnsFalse()
    {
        using (var context = new DefaultContext(_options))
        {
            var repository = new SaleRepository(context);
            var result = await repository.DeleteAsync(Guid.NewGuid());

            Assert.False(result);
        }
    }
}
