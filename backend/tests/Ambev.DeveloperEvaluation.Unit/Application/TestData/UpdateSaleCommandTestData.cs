using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class UpdateSaleCommandTestData
    {
        private static readonly Faker<UpdateSaleCommand> CommandFaker = new Faker<UpdateSaleCommand>()
            .RuleFor(c => c.Id, f => Guid.NewGuid())
            .RuleFor(c => c.Date, f => f.Date.Recent())
            .RuleFor(c => c.CustomerId, f => Guid.NewGuid())
            .RuleFor(c => c.CustomerName, f => f.Name.FullName())
            .RuleFor(c => c.BranchId, f => Guid.NewGuid())
            .RuleFor(c => c.BranchName, f => f.Company.CompanyName())
            .RuleFor(c => c.Items, f => Enumerable.Range(1, f.Random.Number(1, 5))
                .Select(_ => new UpdateSaleItemCommand
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    ProductName = f.Commerce.ProductName(),
                    Quantity = f.Random.Number(1, 10),
                    UnitPrice = f.Random.Decimal(10, 100)
                })
                .ToList());

        public static UpdateSaleCommand GenerateValidCommand()
        {
            return CommandFaker.Generate();
        }
    }