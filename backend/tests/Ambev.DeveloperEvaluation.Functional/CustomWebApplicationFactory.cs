using Ambev.DeveloperEvaluation.ORM;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Functional;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<DefaultContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            var redisDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDistributedCache));
            if (redisDescriptor != null)
            {
                services.Remove(redisDescriptor);
            }

            services.AddDistributedMemoryCache();

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

            try
            {
                db.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred creating the test database. Error: {Message}", ex.Message);
                throw;
            }
        });

        builder.UseEnvironment("Development");
    }
}