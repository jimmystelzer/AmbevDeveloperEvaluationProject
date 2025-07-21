using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetSaleHandlerTests
{
    private readonly ISaleService _saleService;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;
    private readonly ICacheService _cacheService;

    public GetSaleHandlerTests()
    {
        _saleService = Substitute.For<ISaleService>();
        _mapper = Substitute.For<IMapper>();
        _cacheService = Substitute.For<ICacheService>();
        _handler = new GetSaleHandler(_saleService, _mapper, _cacheService);
    }

    [Fact]
    public async Task Handle_ValidId_ReturnsSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);
        var sale = SaleTestData.GenerateValidSale();
        sale.Id = saleId;

        var expectedResult = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            Date = sale.Date,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            BranchId = sale.BranchId,
            BranchName = sale.BranchName,
            TotalAmount = sale.TotalAmount,
            Status = sale.Status,
            Items = new List<GetSaleItemResult>()
        };

        _saleService.GetSaleByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(saleId);
        result.SaleNumber.Should().Be(sale.SaleNumber);
        await _saleService.Received(1).GetSaleByIdAsync(saleId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);

        _saleService.GetSaleByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found");
    }

    [Fact]
    public async Task Handle_InvalidId_ThrowsValidationException()
    {
        // Arrange
        var command = new GetSaleCommand(Guid.Empty);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}