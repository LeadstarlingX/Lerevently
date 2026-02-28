using System.Data.Common;
using Dapper;
using Lerevently.Common.Application.Data;
using Lerevently.Common.Application.EventBus;
using Lerevently.Common.Infrastructure.Inbox;

namespace Lerevently.Modules.Ticketing.Infrastructure.Inbox;

internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent>(
    IIntegrationEventHandler<TIntegrationEvent> decorated,
    IDbConnectionFactory dbConnectionFactory)
    : IntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    public override async Task Handle(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        await using DbConnection connection = await dbConnectionFactory.GetDbConnectionAsync();

        var inboxMessageConsumer = new InboxMessageConsumer(integrationEvent.Id, decorated.GetType().Name);

        if (await InboxConsumerExistsAsync(connection, inboxMessageConsumer))
        {
            return;
        }

        await decorated.Handle(integrationEvent, cancellationToken);

        await InsertInboxConsumerAsync(connection, inboxMessageConsumer);
    }

    private static async Task<bool> InboxConsumerExistsAsync(
        DbConnection dbConnection,
        InboxMessageConsumer inboxMessageConsumer)
    {
        const string sql =
            """
            SELECT EXISTS(
                SELECT 1
                FROM ticketing."InboxMessageConsumer";
                WHERE "InboxMessageId" = @InboxMessageId AND
                      "Name" = @Name
            )
            """;

        return await dbConnection.ExecuteScalarAsync<bool>(sql, inboxMessageConsumer);
    }

    private static async Task InsertInboxConsumerAsync(
        DbConnection dbConnection,
        InboxMessageConsumer inboxMessageConsumer)
    {
        const string sql =
            """
            INSERT INTO ticketing."InboxMessageConsumer"("InboxMessageId", "Name")
            VALUES (@InboxMessageId, @Name)
            """;

        await dbConnection.ExecuteAsync(sql, inboxMessageConsumer);
    }
}
