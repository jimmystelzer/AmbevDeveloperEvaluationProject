using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

namespace Ambev.DeveloperEvaluation.Application.Mappings;


public class SaleProfile : Profile
{
    public SaleProfile()
    {
        // Domain -> Application mappings for GetSale
        CreateMap<Sale, GetSaleResult>();
        CreateMap<SaleItem, GetSaleItemResult>();

        // Domain -> Application mappings for GetSales
        CreateMap<Sale, GetSalesItemResult>()
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.Items.Count));

        // Domain -> Application mappings for CreateSale
        CreateMap<Sale, CreateSaleResult>();
        CreateMap<SaleItem, SaleItemResult>();

        // Domain -> Application mappings for UpdateSale
        CreateMap<Sale, UpdateSaleResult>();
        CreateMap<SaleItem, UpdateSaleItemResult>();
    }
}