using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Sales.EventHandlers;
using Ambev.DeveloperEvaluation.Domain.Services.DiscountStrategies;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class SalesModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        // Registrar o repositório
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();
        
        // Registrar estratégias de desconto
        builder.Services.AddTransient<IDiscountStrategy, NoDiscountStrategy>();
        builder.Services.AddTransient<IDiscountStrategy, TenPercentDiscountStrategy>();
        builder.Services.AddTransient<IDiscountStrategy, TwentyPercentDiscountStrategy>();
        
        // Registrar specifications
        builder.Services.AddTransient<ISpecification<Sale>, ValidSaleSpecification>();
        builder.Services.AddTransient<ISpecification<Sale>, ActiveSaleSpecification>();
        builder.Services.AddTransient<ISpecification<SaleItem>, SaleItemQuantitySpecification>();
        builder.Services.AddTransient<ISpecification<SaleItem>, SaleItemDiscountEligibilitySpecification>();
        
        // Registrar serviços
        builder.Services.AddScoped<ISaleDiscountService, DefaultSaleDiscountService>();
        builder.Services.AddScoped<IEventService, MediatRDomainEventService>();
        builder.Services.AddScoped<ISaleService, SaleService>();
        
        // Registrar handlers de eventos
        builder.Services.AddTransient<INotificationHandler<SaleCreatedEvent>, SaleCreatedEventHandler>();
        builder.Services.AddTransient<INotificationHandler<SaleModifiedEvent>, SaleModifiedEventHandler>();
        builder.Services.AddTransient<INotificationHandler<SaleCancelledEvent>, SaleCancelledEventHandler>();
        builder.Services.AddTransient<INotificationHandler<ItemCancelledEvent>, ItemCancelledEventHandler>();
        
    }
}