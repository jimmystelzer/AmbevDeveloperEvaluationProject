using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public string SaleNumber { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public Guid BranchId { get; private set; }
    public string BranchName { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public SaleStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    public List<SaleItem> Items { get; private set; } = new List<SaleItem>();
    
    protected Sale() { }
    
    public Sale(
        Guid id,
        string saleNumber,
        DateTime date,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName)
    {
        Id = id;
        SaleNumber = saleNumber;
        Date = date;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        Status = SaleStatus.Active;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        TotalAmount = 0;
    }
    
    public void AddItem(SaleItem item)
    {
        Items.Add(item);
        CalculateTotalAmount();
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Cancel()
    {
        if (Status == SaleStatus.Cancelled)
            throw new InvalidOperationException("Sale is already cancelled");
            
        Status = SaleStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
    
    private void CalculateTotalAmount()
    {
        TotalAmount = Items?.Sum(item => item?.TotalAmount ?? 0) ?? 0;
    }
}
