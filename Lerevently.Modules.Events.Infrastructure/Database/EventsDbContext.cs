using Lerevently.Modules.Events.Application.Abstractions.Data;
using Lerevently.Modules.Events.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Lerevently.Modules.Events.Infrastructure.Database
{
    public sealed class EventsDbContext(DbContextOptions<EventsDbContext> options) : DbContext(options), IUnitOfWork
    {

        internal DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schemas.Events);



            base.OnModelCreating(modelBuilder);
        }

    }
}
