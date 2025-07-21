using Xunit;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using FluentValidation.TestHelper;
using System;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users
{
    public class DeleteUserValidatorTests
    {
        private readonly DeleteUserValidator _validator;

        public DeleteUserValidatorTests()
        {
            _validator = new DeleteUserValidator();
        }

        [Fact]
        public void Should_have_error_when_Id_is_empty()
        {
            _validator.TestValidate(new DeleteUserCommand(Guid.Empty))
                .ShouldHaveValidationErrorFor(command => command.Id);
        }

        [Fact]
        public void Should_not_have_error_when_Id_is_not_empty()
        {
            _validator.TestValidate(new DeleteUserCommand(Guid.NewGuid()))
                .ShouldNotHaveValidationErrorFor(command => command.Id);
        }
    }
}