namespace Ambev.DeveloperEvaluation.Domain.Services.DiscountStrategies;
public class TenPercentDiscountStrategy : IDiscountStrategy
{
    public bool IsApplicable(int quantity) => quantity >= 4 && quantity < 10;
    
    public decimal CalculateDiscount(int quantity, decimal unitPrice) => unitPrice * quantity * 0.1m;
}
