using System.Data.Common;
using Lerevently.Common.Application.Data;
using Npgsql;

namespace Lerevently.Common.Infrastructure.Data;

internal sealed class DbConnectionFactory(NpgsqlDataSource dataSource) : IDbConnectionFactory
{
    public async ValueTask<DbConnection> GetDbConnectionAsync()
    {
        return await dataSource.OpenConnectionAsync();
    }
}