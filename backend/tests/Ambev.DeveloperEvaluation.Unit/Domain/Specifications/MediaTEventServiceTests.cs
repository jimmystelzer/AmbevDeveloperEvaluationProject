using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Services;

public class MediatRDomainEventServiceTests
{
    private readonly IMediator _mediator;
    private readonly MediatRDomainEventService _eventService;

    public MediatRDomainEventServiceTests()
    {
        _mediator = Substitute.For<IMediator>();
        _eventService = new MediatRDomainEventService(_mediator);
    }

    [Fact]
    public async Task PublishAsync_ShouldPublishEventViaMediator()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();
        var sale = new Sale(
            Guid.NewGuid(),
            command.SaleNumber,
            command.Date,
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName
        );
        var saleEvent = Substitute.For<SaleCreatedEvent>(sale);
        
        // Act
        await _eventService.PublishAsync(saleEvent);
        
        // Assert
        await _mediator.Received(1).Publish(saleEvent, Arg.Any<CancellationToken>());
    }
}
