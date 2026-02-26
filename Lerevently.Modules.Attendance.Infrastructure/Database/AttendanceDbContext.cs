using Lerevently.Common.Application.Data;
using Lerevently.Modules.Attendance.Application.Abstractions.Data;
using Lerevently.Modules.Attendance.Domain.Attendees;
using Lerevently.Modules.Attendance.Domain.Events;
using Lerevently.Modules.Attendance.Domain.Tickets;
using Lerevently.Modules.Attendance.Infrastructure.Attendees;
using Lerevently.Modules.Attendance.Infrastructure.Events;
using Lerevently.Modules.Attendance.Infrastructure.Tickets;
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AttendanceDbContext).Assembly);
    }
}
