using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleResult
{
    public Guid Id { get; set; }

    public string SaleNumber { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string BranchName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public SaleStatus Status { get; set; }

    public List<UpdateSaleItemResult> Items { get; set; } = new List<UpdateSaleItemResult>();
}

public class UpdateSaleItemResult
{
    public Guid Id { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }

    public decimal TotalAmount { get; set; }
}
