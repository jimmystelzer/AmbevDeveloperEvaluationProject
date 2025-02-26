using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

public class ValidSaleSpecification : CompositeSpecification<Sale>
{
    public override bool IsSatisfiedBy(Sale entity)
    {
        return !string.IsNullOrEmpty(entity.SaleNumber) &&
                !string.IsNullOrEmpty(entity.CustomerName) &&
                !string.IsNullOrEmpty(entity.BranchName) &&
                entity.Items.Any();
    }
}
