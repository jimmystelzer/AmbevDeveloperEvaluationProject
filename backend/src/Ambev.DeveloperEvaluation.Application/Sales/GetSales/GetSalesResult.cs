using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesResult
{
    public List<GetSalesItemResult> Sales { get; set; } = new List<GetSalesItemResult>();

    public int TotalCount { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalPages { get; set; }
}

public class GetSalesItemResult
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
