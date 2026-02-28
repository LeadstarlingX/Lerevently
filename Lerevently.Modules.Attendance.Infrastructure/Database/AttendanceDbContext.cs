using Lerevently.Common.Infrastructure.Inbox;
using Lerevently.Common.Infrastructure.Outbox;
using Lerevently.Modules.Attendance.Domain.Attendees;
using Lerevently.Modules.Attendance.Domain.Events;
using Lerevently.Modules.Attendance.Domain.Tickets;
using Microsoft.EntityFrameworkCore;
using IUnitOfWork = Lerevently.Modules.Attendance.Application.Abstractions.Data.IUnitOfWork;

namespace Lerevently.Modules.Attendance.Infrastructure.Database;

public sealed class AttendanceDbContext(DbContextOptions<AttendanceDbContext> options)
    : DbContext(options), IUnitOfWork
{
    internal DbSet<Attendee> Attendees { get; set; }

    internal DbSet<Event> Events { get; set; }

    internal DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Attendance);

        
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AttendanceDbContext).Assembly);
    }
}