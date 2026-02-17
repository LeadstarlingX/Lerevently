using Lerevently.Api.Extenstions;
using Lerevently.Common.Application;
using Lerevently.Common.Infrastructure;
using Lerevently.Modules.Events.Application;
using Lerevently.Modules.Events.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

namespace Lerevently.Api;

internal class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }
    
    public void ConfigureServices(IServiceCollection services)
    {
        var conn = $"Connection String: {Configuration.GetConnectionString("Database")}";
        Console.WriteLine($"\n******** Using connection string: {conn} ********\n");

        services.AddApplication([AssemblyReference.Assembly])
            .AddInfrastructure(Configuration)
            .AddEventsModule(Configuration)
            .AddApi(Configuration)
            .AddControllers();
        
    }
    
    
    
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
        
        app.ApplyMigrations();
        
        // app.SeedData();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            
        }
        
        app.UseRouting();
        
        
        app.UseEndpoints(endpoints =>
        {
            EventsModule.MapEndpoints(endpoints);  // endpoints is IEndpointRouteBuilder
        });

        app.UseSerilogRequestLogging();

        app.UseExceptionHandler();

        // app.UseHttpsRedirection();

        /*
         app.UseStaticFiles();


        app.UseAuthentication();

        app.UseAuthorization();
        */

    }
    
}