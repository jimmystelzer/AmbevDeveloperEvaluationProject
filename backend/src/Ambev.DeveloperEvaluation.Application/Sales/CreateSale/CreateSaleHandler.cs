using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleService _saleService;
    private readonly IMapper _mapper;
    
    public CreateSaleHandler(
        ISaleService saleService,
        IMapper mapper)
    {
        _saleService = saleService;
        _mapper = mapper;
    }
    
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
            
        if (string.IsNullOrEmpty(command.SaleNumber))
        {
            command.SaleNumber = $"SALE-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
        
        var saleId = Guid.NewGuid();
        
        var sale = new Sale(
            saleId,
            command.SaleNumber,
            command.Date,
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName
        );
        
        foreach (var itemCommand in command.Items)
        {
            var item = _saleService.CreateSaleItem(
                saleId,
                itemCommand.ProductId,
                itemCommand.ProductName,
                itemCommand.Quantity,
                itemCommand.UnitPrice
            );
            
            sale.AddItem(item);
        }
        
        var createdSale = await _saleService.CreateSaleAsync(sale, cancellationToken);
        
        return _mapper.Map<CreateSaleResult>(createdSale);
    }
}