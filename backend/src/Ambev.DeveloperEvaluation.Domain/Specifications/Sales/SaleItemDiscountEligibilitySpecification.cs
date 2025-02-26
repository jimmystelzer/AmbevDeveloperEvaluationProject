using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

public class SaleItemDiscountEligibilitySpecification : CompositeSpecification<SaleItem>
{
    public override bool IsSatisfiedBy(SaleItem entity)
    {
        return entity.Quantity >= 4;
    }
}
