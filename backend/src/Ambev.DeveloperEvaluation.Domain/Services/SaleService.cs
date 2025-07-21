using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public class SaleService : ISaleService
{
    private readonly ISaleDiscountService _discountService;
    private readonly ISaleRepository _saleRepository;
    private readonly ISpecification<Sale> _validSaleSpecification;
    private readonly ISpecification<Sale> _activeSaleSpecification;
    private readonly ISpecification<SaleItem> _validQuantitySpecification;
    private readonly IEventService _eventService;

    private readonly ILogger<SaleService> _logger;

    public SaleService(
        ISaleDiscountService discountService,
        ISaleRepository saleRepository,
        IEventService eventService,
        ILogger<SaleService> logger)
    {
        _discountService = discountService;
        _saleRepository = saleRepository;
        _validSaleSpecification = new ValidSaleSpecification();
        _activeSaleSpecification = new ActiveSaleSpecification();
        _logger = logger;
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
        _logger.LogDebug("Creating SaleItem: {@SaleItem}", item);

        if (!_validQuantitySpecification.IsSatisfiedBy(item))
        {
            _logger.LogWarning("Invalid quantity for SaleItem. Quantity: {Quantity}, Max allowed: 20", quantity);
            throw new ArgumentException($"Invalid quantity: {quantity}. Maximum allowed is 20.");
        }

        _logger.LogInformation("SaleItem created successfully: {@SaleItem}", item);
        return item;
    }

    public bool IsValidSale(Sale sale, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (!_validSaleSpecification.IsSatisfiedBy(sale))
        {
            errorMessage = "Sale must have valid number, customer, branch and at least one item";
            _logger.LogWarning("Sale validation failed: {ErrorMessage}. Sale: {@Sale}", errorMessage, sale);
            return false;
        }

        _logger.LogDebug("Sale validation passed: {@Sale}", sale);
        return true;
    }

    public void ApplyDiscounts(Sale sale)
    {
        foreach (var item in sale.Items)
        {
            var discount = _discountService.CalculateDiscount(item.Quantity, item.UnitPrice);
            var oldPrice = item.UnitPrice;
            item.ApplyDiscount(discount);
            _logger.LogDebug("Applied discount: {Discount} for SaleItem: {@SaleItem}. Old price: {OldPrice}, New price: {NewPrice}", discount, item, oldPrice, item.UnitPrice);
        }
    }

    public async Task<Sale> CreateSaleAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting sale creation: {@Sale}", sale);
        ApplyDiscounts(sale);

        if (!IsValidSale(sale, out var errorMessage))
        {
            _logger.LogError("Sale creation failed validation: {ErrorMessage}. Sale: {@Sale}", errorMessage, sale);
            throw new ArgumentException(errorMessage);
        }

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        _logger.LogInformation("Sale created in repository: {@Sale}", createdSale);

        await _eventService.PublishAsync(new SaleCreatedEvent(createdSale), cancellationToken);
        _logger.LogInformation("SaleCreatedEvent published for sale: {@Sale}", createdSale);

        return createdSale;
    }

    public async Task<Sale?> GetSaleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching sale by ID: {SaleId}", id);
        var sale = await _saleRepository.GetByIdAsync(id, cancellationToken);
        if (sale == null)
            _logger.LogWarning("Sale not found for ID: {SaleId}", id);
        else
            _logger.LogDebug("Sale found: {@Sale}", sale);
        return sale;
    }

    public async Task<Sale?> GetSaleBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching sale by sale number: {SaleNumber}", saleNumber);
        var sale = await _saleRepository.GetBySaleNumberAsync(saleNumber, cancellationToken);
        if (sale == null)
            _logger.LogWarning("Sale not found for sale number: {SaleNumber}", saleNumber);
        else
            _logger.LogDebug("Sale found: {@Sale}", sale);
        return sale;
    }

    public async Task<Sale> UpdateSaleAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting update for sale: {@Sale}", sale);
        var existingSale = await GetSaleByIdAsync(sale.Id, cancellationToken);
        if (existingSale == null)
        {
            _logger.LogError("Sale not found for update. ID: {SaleId}", sale.Id);
            throw new KeyNotFoundException($"Sale with ID {sale.Id} not found");
        }

        if (!_activeSaleSpecification.IsSatisfiedBy(existingSale))
        {
            _logger.LogWarning("Attempt to update inactive sale: {@Sale}", existingSale);
            throw new InvalidOperationException("Cannot update a sale that is not active");
        }

        ApplyDiscounts(sale);

        if (!IsValidSale(sale, out var errorMessage))
        {
            _logger.LogError("Sale update failed validation: {ErrorMessage}. Sale: {@Sale}", errorMessage, sale);
            throw new ArgumentException(errorMessage);
        }

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        _logger.LogInformation("Sale updated in repository: {@Sale}", updatedSale);

        await _eventService.PublishAsync(new SaleModifiedEvent(updatedSale), cancellationToken);
        _logger.LogInformation("SaleModifiedEvent published for sale: {@Sale}", updatedSale);

        return updatedSale;
    }

    public async Task<bool> DeleteSaleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting cancellation for sale ID: {SaleId}", id);
        var sale = await GetSaleByIdAsync(id, cancellationToken);
        if (sale == null)
        {
            _logger.LogWarning("Sale not found for cancellation. ID: {SaleId}", id);
            return false;
        }

        if (!_activeSaleSpecification.IsSatisfiedBy(sale))
        {
            _logger.LogWarning("Attempt to cancel inactive sale: {@Sale}", sale);
            throw new InvalidOperationException("Cannot cancel a sale that is not active");
        }

        sale.Cancel();
        await _saleRepository.UpdateAsync(sale, cancellationToken);
        _logger.LogInformation("Sale cancelled in repository: {@Sale}", sale);

        await _eventService.PublishAsync(new SaleCancelledEvent(sale), cancellationToken);
        _logger.LogInformation("SaleCancelledEvent published for sale: {@Sale}", sale);

        return true;
    }

    public async Task<IEnumerable<Sale>> GetAllSalesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching all sales. Page: {Page}, PageSize: {PageSize}", page, pageSize);
        var sales = await _saleRepository.GetAllAsync(page, pageSize, cancellationToken);
        _logger.LogDebug("Fetched {Count} sales.", sales is ICollection<Sale> col ? col.Count : sales.Count());
        return sales;
    }

    public async Task<int> GetSalesCountAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching sales count.");
        var count = await _saleRepository.GetCountAsync(cancellationToken);
        _logger.LogDebug("Sales count: {Count}", count);
        return count;
    }
}
