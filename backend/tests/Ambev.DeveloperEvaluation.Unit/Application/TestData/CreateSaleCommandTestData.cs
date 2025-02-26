using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class CreateSaleCommandTestData
{
    private static readonly Faker<CreateSaleCommand> CommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(c => c.SaleNumber, f => $"SALE-{DateTime.Now:yyyyMMdd}-{f.Random.AlphaNumeric(8)}")
        .RuleFor(c => c.Date, f => f.Date.Recent())
        .RuleFor(c => c.CustomerId, f => Guid.NewGuid())
        .RuleFor(c => c.CustomerName, f => f.Name.FullName())
        .RuleFor(c => c.BranchId, f => Guid.NewGuid())
        .RuleFor(c => c.BranchName, f => f.Company.CompanyName())
        .RuleFor(c => c.Items, f => Enumerable.Range(1, f.Random.Number(1, 5))
            .Select(_ => new CreateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                ProductName = f.Commerce.ProductName(),
                Quantity = f.Random.Number(1, 10),
                UnitPrice = f.Random.Decimal(10, 100)
            })
            .ToList());

    public static CreateSaleCommand GenerateValidCommand()
    {
        return CommandFaker.Generate();
    }
}