using Lerevently.Api;
using Lerevently.Api.Extenstions;
using Lerevently.Modules.Events.Infrastructure;

public static class Program
{
    private static readonly string[] Modules = ["events"]; // Add your module names here

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                // Add module-specific configuration files
                config.AddModuleConfiguration(Modules);
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }

}