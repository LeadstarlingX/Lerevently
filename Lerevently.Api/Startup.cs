using HealthChecks.UI.Client;
using Lerevently.Api.Extensions;
using Lerevently.Common.Application;
using Lerevently.Common.Infrastructure;
using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Attendance.Infrastructure;
using Lerevently.Modules.Events.Application;
using Lerevently.Modules.Events.Infrastructure;
using Lerevently.Modules.Ticketing.Infrastructure;
using Lerevently.Modules.Users.Infrastructure;
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

        conn = $"Redis Connection String: {Configuration.GetConnectionString("Cache")}";

        Console.WriteLine($"\n******** Using connection string: {conn} ********\n");

        services.AddApplication([
            AssemblyReference.Assembly,
            Modules.Users.Application.AssemblyReference.Assembly,
            Modules.Ticketing.Application.AssemblyReference.Assembly,
            Modules.Attendance.Application.AssemblyReference.Assembly
        ]);

        services.AddInfrastructure(
            [
                EventsModule.ConfigureConsumers(Configuration),
                TicketingModule.ConfigureConsumers,
                AttendanceModule.ConfigureConsumers
            ],
            Configuration
        );
        services.AddApi(Configuration);
        services.AddControllers();

        services.AddEventsModule(Configuration);
        services.AddUsersModule(Configuration);
        services.AddTicketingModule(Configuration);
        services.AddAttendanceModule(Configuration);
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();


        app.ApplyMigrations();
        // app.SeedDataAsync().GetAwaiter().GetResult();

        // app.SeedData();

        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();


        app.UseSerilogRequestLogging();

        app.UseExceptionHandler();


        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            
            endpoints.MapEndpoints(); // endpoints is IEndpointRouteBuilder

        });

        // app.UseHttpsRedirection();

        /*
         app.UseStaticFiles();


        app.UseAuthentication();

        app.UseAuthorization();
        */
    }
}