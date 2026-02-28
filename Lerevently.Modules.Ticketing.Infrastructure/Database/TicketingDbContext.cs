using System.Data.Common;
using Lerevently.Common.Infrastructure.Outbox;
using Lerevently.Modules.Ticketing.Application.Abstractions.Data;
using Lerevently.Modules.Ticketing.Domain.Customers;
using Lerevently.Modules.Ticketing.Domain.Events;
using Lerevently.Modules.Ticketing.Domain.Orders;
using Lerevently.Modules.Ticketing.Domain.Payments;
using Lerevently.Modules.Ticketing.Domain.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lerevently.Modules.Ticketing.Infrastructure.Database;

public sealed class TicketingDbContext(DbContextOptions<TicketingDbContext> options)
    : DbContext(options), IUnitOfWork
{
    internal DbSet<Customer> Customers { get; set; }

    internal DbSet<Event> Events { get; set; }

    internal DbSet<TicketType> TicketTypes { get; set; }

    internal DbSet<Order> Orders { get; set; }

    internal DbSet<OrderItem> OrderItems { get; set; }

    internal DbSet<Ticket> Tickets { get; set; }

    internal DbSet<Payment> Payments { get; set; }

    public async Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Database.CurrentTransaction is not null) await Database.CurrentTransaction.DisposeAsync();

        return (await Database.BeginTransactionAsync(cancellationToken)).GetDbTransaction();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Ticketing);

        
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketingDbContext).Assembly);
    }
}