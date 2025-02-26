using Ambev.DeveloperEvaluation.Domain.Services.DiscountStrategies;

namespace Ambev.DeveloperEvaluation.Domain.Services;
public class DefaultSaleDiscountService : ISaleDiscountService
{
    private readonly IEnumerable<IDiscountStrategy> _discountStrategies;
    
    public DefaultSaleDiscountService(IEnumerable<IDiscountStrategy> discountStrategies)
    {
        _discountStrategies = discountStrategies;
    }
    
    public decimal CalculateDiscount(int quantity, decimal unitPrice)
    {
        if (!IsValidQuantity(quantity) || !CanApplyDiscount(quantity))
            return 0;
            
        var strategy = _discountStrategies.FirstOrDefault(s => s.IsApplicable(quantity));
        return strategy?.CalculateDiscount(quantity, unitPrice) ?? 0;
    }
    
    public bool CanApplyDiscount(int quantity) => quantity >= 4;
    
    public bool IsValidQuantity(int quantity) => quantity > 0 && quantity <= 20;
}
