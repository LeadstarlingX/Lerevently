using Lerevently.Modules.Events.Api.Database;
using Lerevently.Modules.Events.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lerevently.Api.Extenstions
{
    internal static class MigrationExtensions
    {

        internal static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            ApplyMigrations<EventsDbContext>(scope);
        }



        private static void ApplyMigrations<TDbContext>(IServiceScope scope) where TDbContext : DbContext
        {
            using var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
            context.Database.Migrate();

        }
    }
}
