using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentValidation;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Application
{
    public class GetSalesHandlerTests
    {
        [Fact]
        public async Task Handle_ValidRequest_ReturnsSalesResult()
        {
            // Arrange
            var saleService = Substitute.For<ISaleService>();
            var mapper = Substitute.For<IMapper>();
            var cacheService = Substitute.For<ICacheService>();

            var request = new GetSalesCommand(1, 10);
            var cancellationToken = CancellationToken.None;

            var sales = new List<Sale> { new Sale(
                Guid.NewGuid(),
                "S123",
                DateTime.UtcNow,
                Guid.NewGuid(),
                "Customer A",
                Guid.NewGuid(),
                "Branch A"
            ) };

            var totalCount = 20;

            saleService.GetAllSalesAsync(request.Page, request.PageSize, cancellationToken).Returns(Task.FromResult<IEnumerable<Ambev.DeveloperEvaluation.Domain.Entities.Sale>>(sales));
            saleService.GetSalesCountAsync(cancellationToken).Returns(totalCount);

            var salesItemResults = new List<GetSalesItemResult> { new GetSalesItemResult() };
            mapper.Map<List<GetSalesItemResult>>(sales).Returns(salesItemResults);

            var handler = new GetSalesHandler(saleService, mapper, cacheService);

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(salesItemResults, result.Sales);
            Assert.Equal(totalCount, result.TotalCount);
            Assert.Equal(request.Page, result.Page);
            Assert.Equal(request.PageSize, result.PageSize);
            Assert.Equal((int)Math.Ceiling(totalCount / (double)request.PageSize), result.TotalPages);
        }

        [Fact]
        public async Task Handle_InvalidRequest_ThrowsValidationException()
        {
            // Arrange
            var saleService = Substitute.For<ISaleService>();
            var mapper = Substitute.For<IMapper>();
            var cacheService = Substitute.For<ICacheService>();

            var request = new GetSalesCommand(int.MinValue, 101);
            var cancellationToken = CancellationToken.None;

            var handler = new GetSalesHandler(saleService, mapper, cacheService);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(async () => await handler.Handle(request, cancellationToken));
        }
    }
}
