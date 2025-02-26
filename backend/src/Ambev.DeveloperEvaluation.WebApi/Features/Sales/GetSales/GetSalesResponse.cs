using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;

public class GetSalesItemResponse
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;

    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public SaleStatus Status { get; set; }
    public int ItemCount { get; set; }
}
