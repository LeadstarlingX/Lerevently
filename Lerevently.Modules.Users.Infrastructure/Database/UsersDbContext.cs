using Lerevently.Common.Infrastructure.Inbox;
using Lerevently.Common.Infrastructure.Outbox;
using Lerevently.Modules.Users.Application.Abstractions.Data;
using Lerevently.Modules.Users.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Lerevently.Modules.Users.Infrastructure.Database;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options), IUnitOfWork
{
    internal DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Users);


        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
        
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
    }
}