using Xunit;
using NSubstitute;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users
{
    public class GetUserHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCommand_ReturnsGetUserResult()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var mapper = Substitute.For<IMapper>();
            var command = new GetUserCommand(Guid.NewGuid());

            var user = new User
            {
                Id = command.Id,
                Username = "testuser",
                Email = "test@example.com",
                Phone = "+15551234567",
                Status = UserStatus.Active,
                Role = UserRole.Admin
            };

            var getUserResult = new GetUserResult
            {
                Id = user.Id,
                Name = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                Status = user.Status,
                Role = user.Role
            };

            userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<User?>(user));
            mapper.Map<GetUserResult>(user).Returns(getUserResult);

            var handler = new GetUserHandler(userRepository, mapper);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(getUserResult.Id, result.Id);
            Assert.Equal(getUserResult.Name, result.Name);
            await userRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var mapper = Substitute.For<IMapper>();
            var command = new GetUserCommand(Guid.NewGuid());

            userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<User?>(null));

            var handler = new GetUserHandler(userRepository, mapper);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
            await userRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_InvalidCommand_ThrowsValidationException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var mapper = Substitute.For<IMapper>();
            var command = new GetUserCommand(Guid.Empty);

            var handler = new GetUserHandler(userRepository, mapper);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, CancellationToken.None));
            await userRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }
    }
}