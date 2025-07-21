using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Common.Validation;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class CreateSaleCommandValidatorTests
    {
        [Fact]
        public void Validate_ValidCreateSaleCommand_ReturnsValidResult()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                SaleNumber = "123",
                Date = DateTime.Now,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                BranchId = Guid.NewGuid(),
                BranchName = "Test Branch",
                Items = new List<CreateSaleItemCommand>
                {
                    new CreateSaleItemCommand
                    {
                        ProductId = Guid.NewGuid(),
                        ProductName = "Test Product",
                        Quantity = 1,
                        UnitPrice = 10.00M
                    }
                }
            };

            // Act
            var result = command.Validate();

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();

        }

        [Fact]
        public void Validate_EmptySaleNumber_ReturnsValidResult()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                SaleNumber = string.Empty,
                Date = DateTime.Now,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                BranchId = Guid.NewGuid(),
                BranchName = "Test Branch",
                Items = new List<CreateSaleItemCommand>
                {
                    new CreateSaleItemCommand
                    {
                        ProductId = Guid.NewGuid(),
                        ProductName = "Test Product",
                        Quantity = 1,
                        UnitPrice = 10.00M
                    }
                }
            };

            // Act
            var result = command.Validate();

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_EmptyCustomerName_ReturnsInvalidResult()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                SaleNumber = "123",
                Date = DateTime.Now,
                CustomerId = Guid.NewGuid(),
                CustomerName = string.Empty,
                BranchId = Guid.NewGuid(),
                BranchName = "Test Branch",
                Items = new List<CreateSaleItemCommand>
                {
                    new CreateSaleItemCommand
                    {
                        ProductId = Guid.NewGuid(),
                        ProductName = "Test Product",
                        Quantity = 1,
                        UnitPrice = 10.00M
                    }
                }
            };

            // Act
            var result = command.Validate();

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().AllBeOfType<ValidationErrorDetail>();
        }

        [Fact]
        public void Validate_EmptyBranchName_ReturnsInvalidResult()
        {
            // Arrange
            var command = new CreateSaleCommand
            {
                SaleNumber = "123",
                Date = DateTime.Now,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                BranchId = Guid.NewGuid(),
                BranchName = string.Empty,
                Items = new List<CreateSaleItemCommand>
                {
                    new CreateSaleItemCommand
                    {
                        ProductId = Guid.NewGuid(),
                        ProductName = "Test Product",
                        Quantity = 1,
                        UnitPrice = 10.00M
                    }
                }
            };

            // Act
            var result = command.Validate();

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();

        }
    }
}