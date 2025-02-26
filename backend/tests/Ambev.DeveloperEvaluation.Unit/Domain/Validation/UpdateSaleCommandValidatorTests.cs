using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class UpdateSaleCommandValidatorTests
{
    private readonly UpdateSaleValidator _validator;

    public UpdateSaleCommandValidatorTests()
    {
        _validator = new UpdateSaleValidator();
    }

    [Fact]
    public void Validate_ShouldPassForValidCommand()
    {
        // Arrange
        var command = UpdateSaleCommandTestData.GenerateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFailForEmptyId()
    {
        // Arrange
        var command = UpdateSaleCommandTestData.GenerateValidCommand();
        command.Id = Guid.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_ShouldFailForEmptyCustomerId()
    {
        // Arrange
        var command = UpdateSaleCommandTestData.GenerateValidCommand();
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
        var command = UpdateSaleCommandTestData.GenerateValidCommand();
        command.CustomerName = invalidName;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerName);
    }

    [Fact]
    public void Validate_ShouldFailForEmptyBranchId()
    {
        // Arrange
        var command = UpdateSaleCommandTestData.GenerateValidCommand();
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
        var command = UpdateSaleCommandTestData.GenerateValidCommand();
        command.BranchName = invalidName;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BranchName);
    }

    [Fact]
    public void Validate_ShouldFailForEmptyItems()
    {
        // Arrange
        var command = UpdateSaleCommandTestData.GenerateValidCommand();
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
        var command = UpdateSaleCommandTestData.GenerateValidCommand();
        command.Items.First().Quantity = 21; // Over maximum

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }
}