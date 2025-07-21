using Xunit;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using FluentValidation.TestHelper;
using System;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users
{
    public class GetUserValidatorTests
    {
        private readonly GetUserValidator _validator;

        public GetUserValidatorTests()
        {
            _validator = new GetUserValidator();
        }

        [Fact]
        public void Should_have_error_when_Id_is_empty()
        {
            _validator.TestValidate(new GetUserCommand(Guid.Empty))
                .ShouldHaveValidationErrorFor(command => command.Id);
        }

        [Fact]
        public void Should_not_have_error_when_Id_is_not_empty()
        {
            _validator.TestValidate(new GetUserCommand(Guid.NewGuid()))
                .ShouldNotHaveValidationErrorFor(command => command.Id);
        }
    }
}