using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Services;
using Microsoft.Extensions.Configuration;
using ConfigurationManager = Ambev.DeveloperEvaluation.Common.ConfigurationManager;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesHandler : IRequestHandler<GetSalesCommand, GetSalesResult>
{
    private readonly ISaleService _saleService;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public GetSalesHandler(
        ISaleService saleService,
        IMapper mapper,
        ICacheService cacheService
    )
    {
        _saleService = saleService;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<GetSalesResult> Handle(GetSalesCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetSalesValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var cacheKey = $"GetSales_{request.Page}_{request.PageSize}";
        var cachedResult = await _cacheService.GetAsync<GetSalesResult>(cacheKey);

        if (cachedResult is not null)
        {
            return cachedResult;
        }

        var sales = await _saleService.GetAllSalesAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await _saleService.GetSalesCountAsync(cancellationToken);


        var result = new GetSalesResult
        {
            Sales = _mapper.Map<List<GetSalesItemResult>>(sales),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };

        await _cacheService.SetAsync(cacheKey, result, ConfigurationManager.AppSetting.GetValue<TimeSpan>("CacheSettings:DefaultExpiration"));
        await _cacheService.AddToListAsync("GetSales", cacheKey);

        return result;
    }
}
