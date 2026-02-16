using System.Data.Common;

namespace Lerevently.Common.Application.Data;

public interface IDbConnectionFactory
{
    ValueTask<DbConnection> GetDbConnectionAsync();
}