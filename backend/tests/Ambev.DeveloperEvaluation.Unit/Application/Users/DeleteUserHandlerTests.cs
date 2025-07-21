using Xunit;
using NSubstitute;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users
{
    public class DeleteUserHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccess()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var command = new DeleteUserCommand(Guid.NewGuid());

            userRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

            var handler = new DeleteUserHandler(userRepository);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            await userRepository.Received(1).DeleteAsync(command.Id, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var command = new DeleteUserCommand(Guid.NewGuid());

            userRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

            var handler = new DeleteUserHandler(userRepository);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
            await userRepository.Received(1).DeleteAsync(command.Id, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_InvalidCommand_ThrowsValidationException()
        {
            // Arrange
            var userRepository = Substitute.For<IUserRepository>();
            var command = new DeleteUserCommand(Guid.Empty);

            var handler = new DeleteUserHandler(userRepository);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, CancellationToken.None));
            await userRepository.DidNotReceive().DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }
    }
}