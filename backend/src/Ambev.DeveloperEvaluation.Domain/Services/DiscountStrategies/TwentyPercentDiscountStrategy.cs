namespace Ambev.DeveloperEvaluation.Domain.Services.DiscountStrategies;

public class TwentyPercentDiscountStrategy : IDiscountStrategy
{
    public bool IsApplicable(int quantity) => quantity >= 10 && quantity <= 20;
    
    public decimal CalculateDiscount(int quantity, decimal unitPrice) => unitPrice * quantity * 0.2m;
}