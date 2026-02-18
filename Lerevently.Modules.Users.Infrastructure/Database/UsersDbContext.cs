using Lerevently.Common.Application.Data;
using Lerevently.Modules.Users.Domain.Users;
using Lerevently.Modules.Users.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;

namespace Lerevently.Modules.Users.Infrastructure.Database;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options), IUnitOfWork
{
    internal DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Users);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
