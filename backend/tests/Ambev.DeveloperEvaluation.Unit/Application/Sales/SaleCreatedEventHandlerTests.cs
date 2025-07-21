using Ambev.DeveloperEvaluation.Application.Sales.EventHandlers;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class SaleCreatedEventHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldLogInformation()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SaleCreatedEventHandler>>();
            var handler = new SaleCreatedEventHandler(logger);
            var sale = new Sale(
                Guid.NewGuid(),
                "123",
                DateTime.UtcNow,
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch"
            );
            var saleCreatedEvent = new SaleCreatedEvent(sale);

            // Act
            await handler.Handle(saleCreatedEvent, CancellationToken.None);

            // Assert
            logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }
        
        [Fact]
        public async Task Handle_SaleModifiedEvent_ShouldLogInformation()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SaleModifiedEventHandler>>();
            var handler = new SaleModifiedEventHandler(logger);
            var sale = new Sale(
                Guid.NewGuid(),
                "123",
                DateTime.UtcNow,
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch"
            );
            var saleModifiedEvent = new SaleModifiedEvent(sale);

            // Act
            await handler.Handle(saleModifiedEvent, CancellationToken.None);

            // Assert
            logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }
        
        [Fact]
        public async Task Handle_SaleCancelledEvent_ShouldLogInformation()
        {
            // Arrange
            var logger = Substitute.For<ILogger<SaleCancelledEventHandler>>();
            var handler = new SaleCancelledEventHandler(logger);
            var sale = new Sale(
                Guid.NewGuid(),
                "123",
                DateTime.UtcNow,
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch"
            );
            var saleCancelledEvent = new SaleCancelledEvent(sale);

            // Act
            await handler.Handle(saleCancelledEvent, CancellationToken.None);

            // Assert
            logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }
        
        [Fact]
        public async Task Handle_ItemCancelledEvent_ShouldLogInformation()
        {
            // Arrange
            var logger = Substitute.For<ILogger<ItemCancelledEventHandler>>();
            var handler = new ItemCancelledEventHandler(logger);
            var sale = new Sale(
                Guid.NewGuid(),
                "123",
                DateTime.UtcNow,
                Guid.NewGuid(),
                "Test Customer",
                Guid.NewGuid(),
                "Test Branch"
            );
            var itemCancelledEvent = new ItemCancelledEvent(sale, Guid.NewGuid());

            // Act
            await handler.Handle(itemCancelledEvent, CancellationToken.None);

            // Assert
            logger.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                null,
                Arg.Any<Func<object, Exception?, string>>());
        }
    }
}