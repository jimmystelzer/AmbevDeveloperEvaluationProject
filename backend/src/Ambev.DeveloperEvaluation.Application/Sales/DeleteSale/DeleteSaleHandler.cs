using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, bool>
{
    private readonly ISaleService _saleService;
    private readonly ICacheService _cacheService;

    public DeleteSaleHandler(ISaleService saleService, ICacheService cacheService)
    {
        _saleService = saleService;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var result = await _saleService.DeleteSaleAsync(request.Id, cancellationToken);

        if (result)
        {
            // Invalidate cache for sales list
            await _cacheService.RemoveListAsync("GetSales");
            // Invalidate cache for specific sale
            await _cacheService.RemoveAsync($"GetSale_{request.Id}");
        }

        return result;
    }
}
