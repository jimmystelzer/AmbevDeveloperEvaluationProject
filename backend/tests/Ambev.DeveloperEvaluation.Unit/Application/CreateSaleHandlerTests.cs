using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CreateSaleHandlerTests
{
    private readonly ISaleService _saleService;
    private readonly IMapper _mapper;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleService = Substitute.For<ISaleService>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateSaleHandler(_saleService, _mapper);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
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

        foreach (var itemCommand in command.Items)
        {
            var item = new SaleItem(
                Guid.NewGuid(),
                sale.Id,
                itemCommand.ProductId,
                itemCommand.ProductName,
                itemCommand.Quantity,
                itemCommand.UnitPrice
            );
            sale.AddItem(item);
        }

        var result = new CreateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            Date = sale.Date,
            CustomerName = sale.CustomerName,
            BranchName = sale.BranchName
        };

        _saleService.CreateSaleAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(result);

        // Act
        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Assert
        createSaleResult.Should().NotBeNull();
        createSaleResult.Id.Should().Be(sale.Id);
        createSaleResult.SaleNumber.Should().Be(sale.SaleNumber);
        await _saleService.Received(1).CreateSaleAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = new CreateSaleCommand();

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}