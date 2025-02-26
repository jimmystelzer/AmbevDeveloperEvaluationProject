using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface ISaleService
{
    SaleItem CreateSaleItem(Guid saleId, Guid productId, string productName, int quantity, decimal unitPrice);
    void ApplyDiscounts(Sale sale);
    bool IsValidSale(Sale sale, out string errorMessage);

    Task<Sale> CreateSaleAsync(Sale sale, CancellationToken cancellationToken = default);
    Task<Sale?> GetSaleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sale?> GetSaleBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default);
    Task<Sale> UpdateSaleAsync(Sale sale, CancellationToken cancellationToken = default);
    Task<bool> DeleteSaleAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetAllSalesAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetSalesCountAsync(CancellationToken cancellationToken = default);
    
}
