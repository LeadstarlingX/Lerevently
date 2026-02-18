using Lerevently.Api.Extenstions;
using Serilog;

namespace Lerevently.Api;

public static class Program
{
    private static readonly string[] Modules = ["events", "users"]; // Add your module names here

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration))
            .ConfigureAppConfiguration((context, config) =>
            {
                // Add module-specific configuration files
                config.AddModuleConfiguration(Modules);
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }

}