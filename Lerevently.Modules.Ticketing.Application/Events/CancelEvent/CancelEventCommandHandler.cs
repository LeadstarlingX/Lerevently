using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Ticketing.Application.Abstractions.Data;
using Lerevently.Modules.Ticketing.Domain.Events;

namespace Lerevently.Modules.Ticketing.Application.Events.CancelEvent;

internal sealed class CancelEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<CancelEventCommand>
{
    public async Task<Result> Handle(CancelEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetAsync(request.EventId, cancellationToken);

        if (@event is null) return Result.Failure(EventErrors.NotFound(request.EventId));

        @event.Cancel();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}