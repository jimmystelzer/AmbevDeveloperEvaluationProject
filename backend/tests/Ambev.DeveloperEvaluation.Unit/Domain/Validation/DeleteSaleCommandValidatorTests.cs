using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class DeleteSaleCommandValidatorTests
{
    private readonly DeleteSaleValidator _validator;

    public DeleteSaleCommandValidatorTests()
    {
        _validator = new DeleteSaleValidator();
    }

    [Fact]
    public void Validate_ShouldPassForValidCommand()
    {
        // Arrange
        var command = new DeleteSaleCommand(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFailForEmptyId()
    {
        // Arrange
        var command = new DeleteSaleCommand(Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}