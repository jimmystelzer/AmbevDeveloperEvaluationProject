using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCreatedEvent : INotification
{
    public Sale Sale { get; }
    
    public SaleCreatedEvent(Sale sale)
    {
        Sale = sale;
    }
}

public class SaleModifiedEvent : INotification
{
    public Sale Sale { get; }
    
    public SaleModifiedEvent(Sale sale)
    {
        Sale = sale;
    }
}

public class SaleCancelledEvent : INotification
{
    public Sale Sale { get; }
    
    public SaleCancelledEvent(Sale sale)
    {
        Sale = sale;
    }
}

public class ItemCancelledEvent : INotification
{
    public Sale Sale { get; }
    public Guid ItemId { get; }
    
    public ItemCancelledEvent(Sale sale, Guid itemId)
    {
        Sale = sale;
        ItemId = itemId;
    }
}
