using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConfigurationManager = Ambev.DeveloperEvaluation.Common.ConfigurationManager;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddStackExchangeRedisCache(opt =>
        {
            opt.Configuration = ConfigurationManager.AppSetting.GetConnectionString("RedisCache");
            opt.InstanceName = "AmbevCache:";
        });

        builder.Services.AddScoped<ICacheService, RedisCacheService>();
    }
}