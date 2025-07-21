using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class DeleteSaleResultTests
    {
        [Fact]
        public void DeleteSaleResult_Success_ShouldReturnTrue()
        {
            // Arrange
            var deleteSaleResult = new DeleteSaleResult
            {
                Success = true
            };

            // Act & Assert
            Assert.True(deleteSaleResult.Success);
        }

        [Fact]
        public void DeleteSaleResult_Success_ShouldReturnFalse()
        {
            // Arrange
            var deleteSaleResult = new DeleteSaleResult
            {
                Success = false
            };

            // Act & Assert
            Assert.False(deleteSaleResult.Success);
        }
    }
}