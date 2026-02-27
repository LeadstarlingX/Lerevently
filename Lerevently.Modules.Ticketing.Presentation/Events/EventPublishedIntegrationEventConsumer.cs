using Lerevently.Common.Application.Exceptions;
using Lerevently.Modules.Events.IntegrationEvents;
using Lerevently.Modules.Ticketing.Application.Events.CreateEvent;
using MassTransit;
using MediatR;

namespace Lerevently.Modules.Ticketing.Presentation.Events;

public sealed class EventPublishedIntegrationEventConsumer(ISender sender)
    : IConsumer<EventPublishedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<EventPublishedIntegrationEvent> context)
    {
        var result = await sender.Send(
            new CreateEventCommand(
                context.Message.EventId,
                context.Message.Title,
                context.Message.Description,
                context.Message.Location,
                context.Message.StartsAtUtc,
                context.Message.EndsAtUtc,
                context.Message.TicketTypes
                    .Select(t => new CreateEventCommand.TicketTypeRequest(
                        t.Id,
                        context.Message.EventId,
                        t.Name,
                        t.Price,
                        t.Currency,
                        t.Quantity))
                    .ToList()),
            context.CancellationToken);

        if (result.IsFailure) throw new EventlyException(nameof(CreateEventCommand), result.Error);
    }
}