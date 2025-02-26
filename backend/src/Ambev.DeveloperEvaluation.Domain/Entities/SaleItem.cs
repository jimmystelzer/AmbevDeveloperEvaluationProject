using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    protected SaleItem() { }
    
    public SaleItem(
        Guid id,
        Guid saleId,
        Guid productId,
        string productName,
        int quantity,
        decimal unitPrice)
    {
        Id = id;
        SaleId = saleId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        CalculateTotalAmount();
    }
    
    public void ApplyDiscount(decimal discountAmount)
    {
        Discount = discountAmount;
        CalculateTotalAmount();
        UpdatedAt = DateTime.UtcNow;
    }
    
    private void CalculateTotalAmount()
    {
        TotalAmount = (UnitPrice * Quantity) - Discount;
    }
}
