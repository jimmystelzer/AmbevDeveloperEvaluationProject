using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.EventHandlers;

public class SaleCreatedEventHandler : INotificationHandler<SaleCreatedEvent>
{
    private readonly ILogger<SaleCreatedEventHandler> _logger;
    
    public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Sale created: {SaleId}, Number: {SaleNumber}, Customer: {CustomerName}, Amount: {TotalAmount}",
            notification.Sale.Id,
            notification.Sale.SaleNumber,
            notification.Sale.CustomerName,
            notification.Sale.TotalAmount
        );
        
        return Task.CompletedTask;
    }
}

public class SaleModifiedEventHandler : INotificationHandler<SaleModifiedEvent>
{
    private readonly ILogger<SaleModifiedEventHandler> _logger;
    
    public SaleModifiedEventHandler(ILogger<SaleModifiedEventHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Sale modified: {SaleId}, Number: {SaleNumber}, Customer: {CustomerName}, Amount: {TotalAmount}",
            notification.Sale.Id,
            notification.Sale.SaleNumber,
            notification.Sale.CustomerName,
            notification.Sale.TotalAmount
        );
        
        return Task.CompletedTask;
    }
}

public class SaleCancelledEventHandler : INotificationHandler<SaleCancelledEvent>
{
    private readonly ILogger<SaleCancelledEventHandler> _logger;
    
    public SaleCancelledEventHandler(ILogger<SaleCancelledEventHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Sale cancelled: {SaleId}, Number: {SaleNumber}, Customer: {CustomerName}, Amount: {TotalAmount}",
            notification.Sale.Id,
            notification.Sale.SaleNumber,
            notification.Sale.CustomerName,
            notification.Sale.TotalAmount
        );
        
        return Task.CompletedTask;
    }
}

public class ItemCancelledEventHandler : INotificationHandler<ItemCancelledEvent>
{
    private readonly ILogger<ItemCancelledEventHandler> _logger;
    
    public ItemCancelledEventHandler(ILogger<ItemCancelledEventHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(ItemCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Item cancelled in sale: {SaleId}, Item: {ItemId}",
            notification.Sale.Id,
            notification.ItemId
        );
        
        return Task.CompletedTask;
    }
}