using Lerevently.Common.Application.Exceptions;
using Lerevently.Modules.Events.IntegrationEvents;
using Lerevently.Modules.Ticketing.Application.TicketTypes.UpdateTicketTypePrice;
using MassTransit;
using MediatR;

namespace Lerevently.Modules.Ticketing.Presentation.TicketTypes;

public sealed class TicketTypePriceChangedIntegrationEventConsumer(ISender sender)
    : IConsumer<TicketTypePriceChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<TicketTypePriceChangedIntegrationEvent> context)
    {
        var result = await sender.Send(
            new UpdateTicketTypePriceCommand(context.Message.TicketTypeId, context.Message.Price),
            context.CancellationToken);

        if (result.IsFailure) throw new EventlyException(nameof(UpdateTicketTypePriceCommand), result.Error);
    }
}