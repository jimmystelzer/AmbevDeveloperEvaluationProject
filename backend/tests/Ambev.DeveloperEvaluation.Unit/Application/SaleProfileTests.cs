using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Mappings;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application
{
    public class SaleProfileTests
    {
        [Fact]
        public void SaleProfile_Should_Configure_Mappings()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new SaleProfile());
            });

            configuration.AssertConfigurationIsValid();
        }
    }
}