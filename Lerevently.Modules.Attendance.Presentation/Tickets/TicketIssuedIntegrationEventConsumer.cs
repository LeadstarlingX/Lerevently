using Lerevently.Common.Application.Exceptions;
using Lerevently.Modules.Attendance.Application.Tickets.CreateTicket;
using Lerevently.Modules.Ticketing.IntegrationEvents;
using MassTransit;
using MediatR;

namespace Lerevently.Modules.Attendance.Presentation.Tickets;

public sealed class TicketIssuedIntegrationEventConsumer(ISender sender)
    : IConsumer<TicketIssuedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<TicketIssuedIntegrationEvent> context)
    {
        var result = await sender.Send(
            new CreateTicketCommand(
                context.Message.TicketId,
                context.Message.CustomerId,
                context.Message.EventId,
                context.Message.Code),
            context.CancellationToken);

        if (result.IsFailure) throw new EventlyException(nameof(CreateTicketCommand), result.Error);
    }
}