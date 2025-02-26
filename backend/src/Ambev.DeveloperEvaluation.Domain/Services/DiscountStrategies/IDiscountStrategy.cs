namespace Ambev.DeveloperEvaluation.Domain.Services.DiscountStrategies;

public interface IDiscountStrategy
{
    bool IsApplicable(int quantity);
    decimal CalculateDiscount(int quantity, decimal unitPrice);
}
