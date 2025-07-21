using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class DeleteSaleHandlerTests
{
    private readonly ISaleService _saleService;
    private readonly DeleteSaleHandler _handler;
    private readonly ICacheService _cacheService;

    public DeleteSaleHandlerTests()
    {
        _saleService = Substitute.For<ISaleService>();
        _cacheService = Substitute.For<ICacheService>();
        _handler = new DeleteSaleHandler(_saleService, _cacheService);
    }

    [Fact]
    public async Task Handle_ValidId_ReturnsTrueAndDeletesSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new DeleteSaleCommand(saleId);

        _saleService.DeleteSaleAsync(saleId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        await _saleService.Received(1).DeleteSaleAsync(saleId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonExistentId_ReturnsFalse()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new DeleteSaleCommand(saleId);

        _saleService.DeleteSaleAsync(saleId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        await _saleService.Received(1).DeleteSaleAsync(saleId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidId_ThrowsValidationException()
    {
        // Arrange
        var command = new DeleteSaleCommand(Guid.Empty);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        await _saleService.DidNotReceive().DeleteSaleAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ServiceThrowsException_PropagatesException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new DeleteSaleCommand(saleId);
        var expectedException = new InvalidOperationException("Sale is already cancelled");

        _saleService.DeleteSaleAsync(saleId, Arg.Any<CancellationToken>())
            .Returns<bool>(_ => throw expectedException);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Sale is already cancelled");
        await _saleService.Received(1).DeleteSaleAsync(saleId, Arg.Any<CancellationToken>());
    }
}