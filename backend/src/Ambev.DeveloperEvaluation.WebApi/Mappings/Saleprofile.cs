using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings;

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        // === Request -> Command mappings ===

        // CreateSale request -> command mappings
        CreateMap<CreateSaleRequest, CreateSaleCommand>();
        CreateMap<CreateSaleItemRequest, CreateSaleItemCommand>();

        // GetSale request -> command mappings
        CreateMap<GetSaleRequest, GetSaleCommand>()
            .ConstructUsing(src => new GetSaleCommand(src.Id));

        // UpdateSale request -> command mappings
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
        CreateMap<UpdateSaleItemRequest, UpdateSaleItemCommand>();

        // DeleteSale request -> command mappings
        CreateMap<DeleteSaleRequest, DeleteSaleCommand>()
            .ConstructUsing(src => new DeleteSaleCommand(src.Id));

        // GetSales request -> command mappings
        CreateMap<GetSalesRequest, GetSalesCommand>()
            .ConstructUsing(src => new GetSalesCommand(src.Page, src.PageSize));

        // === Result -> Response mappings ===

        // CreateSale result -> response mappings
        CreateMap<CreateSaleResult, CreateSaleResponse>();
        CreateMap<SaleItemResult, CreateSaleItemResponse>();

        // GetSale result -> response mappings
        CreateMap<GetSaleResult, GetSaleResponse>();
        CreateMap<GetSaleItemResult, GetSaleItemResponse>();

        // UpdateSale result -> response mappings
        CreateMap<UpdateSaleResult, UpdateSaleResponse>();
        CreateMap<UpdateSaleItemResult, UpdateSaleItemResponse>();

        // GetSales result -> response mappings
        CreateMap<GetSalesItemResult, GetSalesItemResponse>();
    }
}