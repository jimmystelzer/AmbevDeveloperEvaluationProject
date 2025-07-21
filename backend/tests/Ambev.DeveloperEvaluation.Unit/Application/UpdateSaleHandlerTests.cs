using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class UpdateSaleHandlerTests
{
    private readonly ISaleService _saleService;
    private readonly IMapper _mapper;
    private readonly UpdateSaleHandler _handler;
    private readonly ICacheService _cacheService;

    public UpdateSaleHandlerTests()
    {
        _saleService = Substitute.For<ISaleService>();
        _mapper = Substitute.For<IMapper>();
        _cacheService = Substitute.For<ICacheService>();
        _handler = new UpdateSaleHandler(_saleService, _mapper, _cacheService);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsUpdatedSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var existingSale = SaleTestData.GenerateValidSale();
        existingSale.Id = saleId;

        var command = UpdateSaleCommandTestData.GenerateValidCommand();
        command.Id = saleId;

        // Mock the new sale that would be created in the handler
        var updatedSale = new Sale(
            saleId,
            existingSale.SaleNumber,
            command.Date,
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName
        );

        var expectedResult = new UpdateSaleResult
        {
            Id = updatedSale.Id,
            SaleNumber = updatedSale.SaleNumber,
            Date = updatedSale.Date,
            CustomerName = updatedSale.CustomerName,
            BranchName = updatedSale.BranchName,
            TotalAmount = updatedSale.TotalAmount,
            Status = updatedSale.Status
        };

        _saleService.GetSaleByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);

        // Setup item creation
        foreach (var itemCommand in command.Items)
        {
            var item = new SaleItem(
                itemCommand.Id ?? Guid.NewGuid(),
                saleId,
                itemCommand.ProductId,
                itemCommand.ProductName,
                itemCommand.Quantity,
                itemCommand.UnitPrice
            );

            _saleService.CreateSaleItem(
                saleId,
                itemCommand.ProductId,
                itemCommand.ProductName,
                itemCommand.Quantity,
                itemCommand.UnitPrice
            ).Returns(item);
        }

        _saleService.UpdateSaleAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(updatedSale);
        _mapper.Map<UpdateSaleResult>(updatedSale).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(saleId);
        result.CustomerName.Should().Be(command.CustomerName);
        await _saleService.Received(1).GetSaleByIdAsync(saleId, Arg.Any<CancellationToken>());
        await _saleService.Received(1).UpdateSaleAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = UpdateSaleCommandTestData.GenerateValidCommand();
        command.Id = saleId;

        _saleService.GetSaleByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found");
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = new UpdateSaleCommand(); // Empty command will fail validation

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}