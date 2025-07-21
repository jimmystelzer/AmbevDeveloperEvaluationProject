using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Services;
using Microsoft.Extensions.Configuration;
using ConfigurationManager = Ambev.DeveloperEvaluation.Common.ConfigurationManager;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler : IRequestHandler<GetSaleCommand, GetSaleResult>
{
    private readonly ISaleService _saleService;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public GetSaleHandler(ISaleService saleService, IMapper mapper, ICacheService cacheService)
    {
        _saleService = saleService;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<GetSaleResult> Handle(GetSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var cacheKey = $"GetSale_{request.Id}";
        var cachedResult = await _cacheService.GetAsync<GetSaleResult>(cacheKey);

        if (cachedResult != null)
            return cachedResult;

        var sale = await _saleService.GetSaleByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        var result = _mapper.Map<GetSaleResult>(sale);
        await _cacheService.SetAsync(cacheKey, result, ConfigurationManager.AppSetting.GetValue<TimeSpan>("CacheSettings:DefaultExpiration"));

        return result;
    }
}
