using Lerevently.Modules.Attendance.Application.Abstractions.Data;
using Lerevently.Common.Infrastructure.Interceptors;
using Lerevently.Common.Presentation.Endpoints;
using Lerevently.Modules.Attendance.Application.Abstractions.Authentication;
using Lerevently.Modules.Attendance.Domain.Attendees;
using Lerevently.Modules.Attendance.Domain.Events;
using Lerevently.Modules.Attendance.Domain.Tickets;
using Lerevently.Modules.Attendance.Infrastructure.Attendees;
using Lerevently.Modules.Attendance.Infrastructure.Authentication;
using Lerevently.Modules.Attendance.Infrastructure.Database;
using Lerevently.Modules.Attendance.Infrastructure.Events;
using Lerevently.Modules.Attendance.Infrastructure.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lerevently.Modules.Attendance.Infrastructure;

public static class AttendanceModule
{
    public static IServiceCollection AddAttendanceModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AttendanceDbContext>((sp, options) =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Attendance))
                .AddInterceptors(sp.GetRequiredService<PublishDomainEventsInterceptor>()));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AttendanceDbContext>());

        services.AddScoped<IAttendeeRepository, AttendeeRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();

        services.AddScoped<IAttendanceContext, AttendanceContext>();
    }
}
