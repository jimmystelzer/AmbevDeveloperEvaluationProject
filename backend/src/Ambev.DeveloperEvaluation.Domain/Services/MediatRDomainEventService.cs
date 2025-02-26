using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public class MediatRDomainEventService : IEventService
{
    private readonly IMediator _mediator;
    
    public MediatRDomainEventService(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : INotification
    {
        await _mediator.Publish(@event, cancellationToken);
    }
}
