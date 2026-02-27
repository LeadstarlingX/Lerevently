using Lerevently.Common.Application.Exceptions;
using Lerevently.Modules.Attendance.Application.Events.CreateEvent;
using Lerevently.Modules.Events.IntegrationEvents;
using MassTransit;
using MediatR;

namespace Lerevently.Modules.Attendance.Presentation.Events;

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
                context.Message.EndsAtUtc),
            context.CancellationToken);

        if (result.IsFailure) throw new EventlyException(nameof(CreateEventCommand), result.Error);
    }
}