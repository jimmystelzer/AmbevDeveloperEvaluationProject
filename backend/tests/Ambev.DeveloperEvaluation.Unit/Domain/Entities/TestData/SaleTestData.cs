using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .CustomInstantiator(f => new Sale(
            Guid.NewGuid(),
            $"SALE-{DateTime.Now:yyyyMMdd}-{f.Random.AlphaNumeric(8)}",
            f.Date.Recent(),
            Guid.NewGuid(),
            f.Name.FullName(),
            Guid.NewGuid(),
            f.Company.CompanyName()
        ));

    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .CustomInstantiator(f => new SaleItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            f.Commerce.ProductName(),
            f.Random.Number(1, 5),
            f.Random.Decimal(10, 100)
        ));

    public static Sale GenerateValidSale()
    {
        return SaleFaker.Generate();
    }

    public static SaleItem GenerateValidSaleItem()
    {
        return SaleItemFaker.Generate();
    }

    public static SaleItem GenerateSaleItemWithQuantity(int quantity)
    {
        return new SaleItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new Faker().Commerce.ProductName(),
            quantity,
            new Faker().Random.Decimal(10, 100)
        );
    }

    public static SaleItem GenerateSaleItemWithValues(decimal unitPrice, int quantity)
    {
        return new SaleItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            new Faker().Commerce.ProductName(),
            quantity,
            unitPrice
        );
    }

    public static List<SaleItem> GenerateMultipleSaleItems(int count)
    {
        return SaleItemFaker.Generate(count);
    }

    public static Sale GenerateSaleWithItems(int itemCount)
    {
        var sale = GenerateValidSale();
        for (int i = 0; i < itemCount; i++)
        {
            var item = GenerateValidSaleItem();
            // Redefine o SaleId para corresponder ao Id da venda
            var newItem = new SaleItem(
                Guid.NewGuid(),
                sale.Id,
                item.ProductId,
                item.ProductName,
                item.Quantity,
                item.UnitPrice
            );
            sale.AddItem(newItem);
        }
        return sale;
    }

    public static Sale GenerateCancelledSale()
    {
        var sale = GenerateValidSale();
        sale.Cancel();
        return sale;
    }
}