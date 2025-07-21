using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Unit.Application.Auth;

public class AuthenticateUserProfileTests
{
    [Fact]
    public void AuthenticateUserProfile_MapsCorrectly()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AuthenticateUserProfile>());
        var mapper = config.CreateMapper();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "Test User",
            Email = "test@example.com",
            Password = "password",
            Phone = "123-456-7890",
            Role = UserRole.Admin,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = mapper.Map<AuthenticateUserResult>(user);

        // Assert
        Assert.Equal(user.Role.ToString(), result.Role);
        Assert.Empty(result.Token);
    }
}