using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CreateSaleCommandValidatorTests
{
    private readonly CreateSaleCommandValidator _validator;

    public CreateSaleCommandValidatorTests()
    {
        _validator = new CreateSaleCommandValidator();
    }

     [Fact]
    public void Validate_ShouldPassForValidCommand()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFailForEmptyCustomerId()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();
        command.CustomerId = Guid.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ShouldFailForEmptyCustomerName(string? invalidName)
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();
        command.CustomerName = invalidName ?? string.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerName);
    }

    [Fact]
    public void Validate_ShouldFailForEmptyBranchId()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();
        command.BranchId = Guid.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BranchId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_ShouldFailForEmptyBranchName(string? invalidName)
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();
        command.BranchName = invalidName ?? string.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BranchName);
    }

    [Fact]
    public void Validate_ShouldFailForEmptyItems()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();
        command.Items.Clear();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void Validate_ShouldFailForItemWithExcessiveQuantity()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();
        command.Items.First().Quantity = 21; // Over maximum

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }

    [Fact]
    public void Validate_ShouldFailForItemWithZeroQuantity()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();
        command.Items.First().Quantity = 0;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }

    [Fact]
    public void Validate_ShouldFailForItemWithNegativeQuantity()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();
        command.Items.First().Quantity = -1;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }

    [Fact]
    public void Validate_ShouldFailForItemWithZeroOrNegativePrice()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();
        command.Items.First().UnitPrice = 0;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].UnitPrice");
    }
}