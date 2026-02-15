using System.Data.Common;

namespace Lerevently.Modules.Events.Application.Abstractions.Data;

public interface IDbConnectionFactory
{
    ValueTask<DbConnection> GetDbConnectionAsync();
}