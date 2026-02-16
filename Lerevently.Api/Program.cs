using Lerevently.Api;
using Lerevently.Api.Extenstions;
using Lerevently.Modules.Events.Infrastructure;

public static class Program
{

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }

}