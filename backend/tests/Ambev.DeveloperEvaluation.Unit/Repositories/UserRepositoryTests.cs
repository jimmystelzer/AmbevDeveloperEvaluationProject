// /home/jimmy/Downloads/Ambev/backend/tests/Ambev.DeveloperEvaluation.Unit/Repositories/UserRepositoryTests.cs
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Unit.Repositories;

public class UserRepositoryTests
{
    private readonly DbContextOptions<DefaultContext> _options;

    public UserRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: "UserRepositoryTests")
            .Options;
    }

    [Fact]
    public async Task CreateAsync_ValidUser_ReturnsCreatedUser()
    {
        using (var context = new DefaultContext(_options))
        {
            var repository = new UserRepository(context);
            var user = new User();
            user.Username = "testuser";
            user.Password = "password";
            user.Email = "test@example.com";
            user.Phone = "123-456-7890";
            user.Status = UserStatus.Active;
            user.Role = UserRole.Customer;

            var createdUser = await repository.CreateAsync(user);

            Assert.NotNull(createdUser);
            Assert.Equal(user.Username, createdUser.Username);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsUser()
    {
        using (var context = new DefaultContext(_options))
        {
            var repository = new UserRepository(context);
            var user = new User();
            user.Username = "testuser";
            user.Password = "password";
            user.Email = "test@example.com";
            user.Phone = "123-456-7890";
            user.Status = UserStatus.Active;
            user.Role = UserRole.Customer;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var retrievedUser = await repository.GetByIdAsync(user.Id);

            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Id, retrievedUser.Id);
        }
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        using (var context = new DefaultContext(_options))
        {
            var repository = new UserRepository(context);
            var retrievedUser = await repository.GetByIdAsync(Guid.NewGuid());

            Assert.Null(retrievedUser);
        }
    }

    [Fact]
    public async Task GetByEmailAsync_ExistingEmail_ReturnsUser()
    {
        using (var context = new DefaultContext(_options))
        {
            var repository = new UserRepository(context);
            var user = new User();
            user.Username = "testuser";
            user.Password = "password";
            user.Email = "test@example.com";
            user.Phone = "123-456-7890";
            user.Status = UserStatus.Active;
            user.Role = UserRole.Customer;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var retrievedUser = await repository.GetByEmailAsync(user.Email);

            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Email, retrievedUser.Email);
        }
    }

    [Fact]
    public async Task GetByEmailAsync_NonExistingEmail_ReturnsNull()
    {
        using (var context = new DefaultContext(_options))
        {
            var repository = new UserRepository(context);
            var retrievedUser = await repository.GetByEmailAsync("nonexistent@example.com");

            Assert.Null(retrievedUser);
        }
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_ReturnsTrue()
    {
        using (var context = new DefaultContext(_options))
        {
            var repository = new UserRepository(context);
            var user = new User();
            user.Username = "testuser";
            user.Password = "password";
            user.Email = "test@example.com";
            user.Phone = "123-456-7890";
            user.Status = UserStatus.Active;
            user.Role = UserRole.Customer;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var result = await repository.DeleteAsync(user.Id);

            Assert.True(result);
            Assert.Null(await repository.GetByIdAsync(user.Id));
        }
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ReturnsFalse()
    {
        using (var context = new DefaultContext(_options))
        {
            var repository = new UserRepository(context);
            var result = await repository.DeleteAsync(Guid.NewGuid());

            Assert.False(result);
        }
    }
}
