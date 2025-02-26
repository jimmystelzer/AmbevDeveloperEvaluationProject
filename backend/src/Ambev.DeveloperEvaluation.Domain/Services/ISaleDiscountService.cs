namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface ISaleDiscountService
{
    decimal CalculateDiscount(int quantity, decimal unitPrice);
    bool CanApplyDiscount(int quantity);
    bool IsValidQuantity(int quantity);
}