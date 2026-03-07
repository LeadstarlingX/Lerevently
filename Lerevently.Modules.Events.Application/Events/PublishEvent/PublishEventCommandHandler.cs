using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Events.Application.Abstractions.Data;
using Lerevently.Modules.Events.Domain.Events;
using Lerevently.Modules.Events.Domain.TicketTypes;

namespace Lerevently.Modules.Events.Application.Events.PublishEvent;

internal sealed class PublishEventCommandHandler(
    IEventRepository eventRepository,
    ITicketTypeRepository ticketTypeRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<PublishEventCommand>
{
    public async Task<Result> Handle(PublishEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetAsync(request.EventId, cancellationToken);

        if (@event is null) return Result.Failure(EventErrors.NotFound(request.EventId));

        if (!await ticketTypeRepository.ExistsAsync(@event.Id, cancellationToken))
            return Result.Failure(EventErrors.NoTicketsFound);

        var result = @event.Publish();

        if (result.IsFailure) return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}