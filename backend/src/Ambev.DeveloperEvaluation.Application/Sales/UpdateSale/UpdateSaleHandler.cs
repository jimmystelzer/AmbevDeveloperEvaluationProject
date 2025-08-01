using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleService _saleService;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public UpdateSaleHandler(
        ISaleService saleService,
        IMapper mapper,
        ICacheService cacheService)
    {
        _saleService = saleService;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleService.GetSaleByIdAsync(command.Id, cancellationToken);
        if (existingSale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        var updatedSale = new Sale(
            command.Id,
            existingSale.SaleNumber,
            command.Date,
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName
        );

        foreach (var itemCommand in command.Items)
        {
            var item = _saleService.CreateSaleItem(
                updatedSale.Id,
                itemCommand.ProductId,
                itemCommand.ProductName,
                itemCommand.Quantity,
                itemCommand.UnitPrice
            );

            updatedSale.AddItem(item);
        }

        var result = await _saleService.UpdateSaleAsync(updatedSale, cancellationToken);

        if (result is not null)
        {
            // Invalidate cache for sales list
            await _cacheService.RemoveListAsync("GetSales");
            // Invalidate cache for specific sale
            await _cacheService.RemoveAsync($"GetSale_{updatedSale.Id}");
        }

        return _mapper.Map<UpdateSaleResult>(result);
    }
}