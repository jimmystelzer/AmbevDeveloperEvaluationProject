using Microsoft.Extensions.Configuration;

namespace Ambev.DeveloperEvaluation.Common;

public static class ConfigurationManager
{
    public static IConfiguration AppSetting { get; }
    static ConfigurationManager()
    {
        AppSetting = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.overrides.json", optional: true, reloadOnChange: true)
            .Build();
    }
}