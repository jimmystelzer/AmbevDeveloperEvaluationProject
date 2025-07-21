using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using FluentValidation.TestHelper;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application
{
    public class AuthenticateUserValidatorTests
    {
        private readonly AuthenticateUserValidator _validator;

        public AuthenticateUserValidatorTests()
        {
            _validator = new AuthenticateUserValidator();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Email_And_password_Are_Valid()
        {
            var command = new AuthenticateUserCommand
            {
                Email = "test@example.com",
                Password = "Password123"
            };

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var command = new AuthenticateUserCommand
            {
                Email = "",
                Password = "Password123"
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var command = new AuthenticateUserCommand
            {
                Email = "testexample.com",
                Password = "Password123"
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            var command = new AuthenticateUserCommand
            {
                Email = "test@example.com",
                Password = ""
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Too_Short()
        {
            var command = new AuthenticateUserCommand
            {
                Email = "test@example.com",
                Password = "Passw"
            };

            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}