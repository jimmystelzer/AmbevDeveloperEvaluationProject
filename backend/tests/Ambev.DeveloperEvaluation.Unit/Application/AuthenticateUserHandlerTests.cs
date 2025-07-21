using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application
{
    public class AuthenticateUserHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var passwordHasher = Substitute.For<IPasswordHasher>();
            var jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();

            var handler = new AuthenticateUserHandler(
                userRepository,
                passwordHasher,
                jwtTokenGenerator);

            var command = new AuthenticateUserCommand
            {
                Email = "test@example.com",
                Password = "password"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "Test User",
                Email = "test@example.com",
                Password = "hashedPassword",
                Role = UserRole.Admin,
                Status = UserStatus.Active
            };

            userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(Task.FromResult<User?>(user));
            passwordHasher.VerifyPassword(command.Password, user.Password).Returns(true);
            jwtTokenGenerator.GenerateToken(user).Returns("generatedToken");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("generatedToken", result.Token);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("Test User", result.Name);
            Assert.Equal(UserRole.Admin.ToString(), result.Role);
        }

        [Fact]
        public async Task Handle_InvalidCredentials_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var passwordHasher = Substitute.For<IPasswordHasher>();
            var jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();

            var handler = new AuthenticateUserHandler(
                userRepository,
                passwordHasher,
                jwtTokenGenerator);

            var command = new AuthenticateUserCommand
            {
                Email = "test@example.com",
                Password = "password"
            };

            userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(Task.FromResult<User?>(null));
            passwordHasher.VerifyPassword(command.Password, Arg.Any<string>()).Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_InactiveUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var passwordHasher = Substitute.For<IPasswordHasher>();
            var jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();

            var handler = new AuthenticateUserHandler(
                userRepository,
                passwordHasher,
                jwtTokenGenerator);

            var command = new AuthenticateUserCommand
            {
                Email = "test@example.com",
                Password = "password"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "Test User",
                Email = "test@example.com",
                Password = "hashedPassword",
                Role = UserRole.Admin,
                Status = UserStatus.Inactive
            };

            userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(Task.FromResult<User?>(user));
            passwordHasher.VerifyPassword(command.Password, user.Password).Returns(true);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
