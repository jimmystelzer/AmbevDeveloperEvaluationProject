using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface IEventService
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : INotification;
}
