using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

public class ActiveSaleSpecification : CompositeSpecification<Sale>
{
    public override bool IsSatisfiedBy(Sale entity)
    {
        return entity.Status == SaleStatus.Active;
    }
}
