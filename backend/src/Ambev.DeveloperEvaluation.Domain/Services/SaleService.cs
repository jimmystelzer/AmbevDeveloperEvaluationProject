using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public class SaleService : ISaleService
{
    private readonly ISaleDiscountService _discountService;
    private readonly ISaleRepository _saleRepository;
    private readonly ISpecification<Sale> _validSaleSpecification;
    private readonly ISpecification<Sale> _activeSaleSpecification;
    private readonly ISpecification<SaleItem> _validQuantitySpecification;
    private readonly IEventService _eventService;
    
    public SaleService(
        ISaleDiscountService discountService,
        ISaleRepository saleRepository,
        IEventService eventService)
    {
        _discountService = discountService;
        _saleRepository = saleRepository;
        _validSaleSpecification = new ValidSaleSpecification();
        _activeSaleSpecification = new ActiveSaleSpecification();
        _validQuantitySpecification = new SaleItemQuantitySpecification();
        _eventService = eventService;
    }
    
    public SaleItem CreateSaleItem(Guid saleId, Guid productId, string productName, int quantity, decimal unitPrice)
    {
        var item = new SaleItem(
            Guid.NewGuid(),
            saleId,
            productId,
            productName,
            quantity,
            unitPrice
        );
        
        if (!_validQuantitySpecification.IsSatisfiedBy(item))
            throw new ArgumentException($"Invalid quantity: {quantity}. Maximum allowed is 20.");
            
        return item;
    }
    
    public bool IsValidSale(Sale sale, out string errorMessage)
    {
        errorMessage = string.Empty;
        
        if (!_validSaleSpecification.IsSatisfiedBy(sale))
        {
            errorMessage = "Sale must have valid number, customer, branch and at least one item";
            return false;
        }
        
        return true;
    }
    
    public void ApplyDiscounts(Sale sale)
    {
        foreach (var item in sale.Items)
        {
            var discount = _discountService.CalculateDiscount(item.Quantity, item.UnitPrice);
            item.ApplyDiscount(discount);
        }
    }

    public async Task<Sale> CreateSaleAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        ApplyDiscounts(sale);
        
        if (!IsValidSale(sale, out var errorMessage))
            throw new ArgumentException(errorMessage);
            
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        
        await _eventService.PublishAsync(new SaleCreatedEvent(createdSale), cancellationToken);
        
        return createdSale;
    }
    
    public async Task<Sale?> GetSaleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _saleRepository.GetByIdAsync(id, cancellationToken);
    }
    
    public async Task<Sale?> GetSaleBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _saleRepository.GetBySaleNumberAsync(saleNumber, cancellationToken);
    }
    
    public async Task<Sale> UpdateSaleAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        var existingSale = await GetSaleByIdAsync(sale.Id, cancellationToken);
        if (existingSale == null)
            throw new KeyNotFoundException($"Sale with ID {sale.Id} not found");
            
        if (!_activeSaleSpecification.IsSatisfiedBy(existingSale))
            throw new InvalidOperationException("Cannot update a sale that is not active");
        
        ApplyDiscounts(sale);
        
        if (!IsValidSale(sale, out var errorMessage))
            throw new ArgumentException(errorMessage);
            
        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        
        await _eventService.PublishAsync(new SaleModifiedEvent(updatedSale), cancellationToken);
        
        return updatedSale;
    }
    
    public async Task<bool> DeleteSaleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetSaleByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;
            
        if (!_activeSaleSpecification.IsSatisfiedBy(sale))
            throw new InvalidOperationException("Cannot cancel a sale that is not active");
            
        sale.Cancel();
        await _saleRepository.UpdateAsync(sale, cancellationToken);
        
        await _eventService.PublishAsync(new SaleCancelledEvent(sale), cancellationToken);
        
        return true;
    }
    
    public async Task<IEnumerable<Sale>> GetAllSalesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _saleRepository.GetAllAsync(page, pageSize, cancellationToken);
    }
    
    public async Task<int> GetSalesCountAsync(CancellationToken cancellationToken = default)
    {
        return await _saleRepository.GetCountAsync(cancellationToken);
    }
}
