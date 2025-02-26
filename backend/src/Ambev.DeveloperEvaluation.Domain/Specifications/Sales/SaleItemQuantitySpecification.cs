using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

public class SaleItemQuantitySpecification : CompositeSpecification<SaleItem>
{
    public override bool IsSatisfiedBy(SaleItem entity)
    {
        return entity.Quantity > 0 && entity.Quantity <= 20;
    }
}
