using Xunit;
using NSubstitute;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Ambev.DeveloperEvaluation.Common.Security;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users
{
    public class CreateUserHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCommand_ReturnsCreateUserResult()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var mapper = Substitute.For<IMapper>();
            var passwordHasher = Substitute.For<IPasswordHasher>();

            var command = new CreateUserCommand
            {
                Username = "testuser",
                Password = "P@sswOrd123",
                Email = "test@example.com",
                Phone = "+15551234567",
                Status = UserStatus.Active,
                Role = UserRole.Admin
            };

            var user = new User { Id = Guid.NewGuid(), Username = command.Username, Email = command.Email };
            var createUserResult = new CreateUserResult { Id = user.Id };

            userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(Task.FromResult<User?>(null));
            passwordHasher.HashPassword(command.Password).Returns("hashed_password");
            mapper.Map<User>(command).Returns(user);
            userRepository.CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(user));
            mapper.Map<CreateUserResult>(user).Returns(createUserResult);

            var handler = new CreateUserHandler(userRepository, mapper, passwordHasher);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(createUserResult.Id, result.Id);
            await userRepository.Received(1).CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ExistingUser_ThrowsException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var mapper = Substitute.For<IMapper>();
            var passwordHasher = Substitute.For<IPasswordHasher>();

            var command = new CreateUserCommand
            {
                Username = "testuser",
                Password = "P@sswOrd123",
                Email = "test@example.com",
                Phone = "+15551234567",
                Status = UserStatus.Active,
                Role = UserRole.Admin
            };

            var existingUser = new User { Id = Guid.NewGuid(), Username = command.Username, Email = command.Email };

            userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(Task.FromResult<User?>(existingUser));

            var handler = new CreateUserHandler(userRepository, mapper, passwordHasher);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
            await userRepository.DidNotReceive().CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_InvalidCommand_ThrowsValidationException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var mapper = Substitute.For<IMapper>();
            var passwordHasher = Substitute.For<IPasswordHasher>();

            var command = new CreateUserCommand
            {
                Username = "te",
                Password = "password",
                Email = "testexample.com",
                Phone = "15551234567",
                Status = UserStatus.Unknown,
                Role = UserRole.None
            };

            var handler = new CreateUserHandler(userRepository, mapper, passwordHasher);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, CancellationToken.None));
            await userRepository.DidNotReceive().CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        }
    }
}