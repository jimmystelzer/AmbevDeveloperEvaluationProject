using Xunit;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using FluentValidation.TestHelper;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users
{
    public class CreateUserValidatorTests
    {
        private readonly CreateUserCommandValidator _validator;

        public CreateUserValidatorTests()
        {
            _validator = new CreateUserCommandValidator();
        }

        [Fact]
        public void Should_have_error_when_Email_is_invalid()
        {
            _validator.TestValidate(new CreateUserCommand { Email = "test" })
                .ShouldHaveValidationErrorFor(user => user.Email);
        }

        [Fact]
        public void Should_have_error_when_Username_is_empty()
        {
            _validator.TestValidate(new CreateUserCommand { Username = "" })
                .ShouldHaveValidationErrorFor(user => user.Username);
        }

        [Fact]
        public void Should_have_error_when_Username_is_too_short()
        {
            _validator.TestValidate(new CreateUserCommand { Username = "te" })
                .ShouldHaveValidationErrorFor(user => user.Username);
        }

        [Fact]
        public void Should_have_error_when_Username_is_too_long()
        {
            _validator.TestValidate(new CreateUserCommand { Username = new string('a', 51) })
                .ShouldHaveValidationErrorFor(user => user.Username);
        }

        [Fact]
        public void Should_have_error_when_Password_is_invalid()
        {
            _validator.TestValidate(new CreateUserCommand { Password = "weak" })
                .ShouldHaveValidationErrorFor(user => user.Password);
        }

        [Fact]
        public void Should_have_error_when_Phone_is_invalid()
        {
            _validator.TestValidate(new CreateUserCommand { Phone = "abc" })
                .ShouldHaveValidationErrorFor(user => user.Phone);
        }

        [Fact]
        public void Should_have_error_when_Status_is_Unknown()
        {
            _validator.TestValidate(new CreateUserCommand { Status = UserStatus.Unknown })
                .ShouldHaveValidationErrorFor(user => user.Status);
        }

        [Fact]
        public void Should_have_error_when_Role_is_None()
        {
            _validator.TestValidate(new CreateUserCommand { Role = UserRole.None })
                .ShouldHaveValidationErrorFor(user => user.Role);
        }

        [Fact]
        public void Should_not_have_error_when_command_is_valid()
        {
            var command = new CreateUserCommand
            {
                Username = "testuser",
                Password = "P@sswOrd123",
                Email = "test@example.com",
                Phone = "+15551234567",
                Status = UserStatus.Active,
                Role = UserRole.Admin
            };

            _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
        }
    }
}