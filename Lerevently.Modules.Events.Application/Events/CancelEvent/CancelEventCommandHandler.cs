using Lerevently.Modules.Events.Application.Abstractions.Clock;
using Lerevently.Modules.Events.Application.Abstractions.Data;
using Lerevently.Modules.Events.Application.Abstractions.Messaging;
using Lerevently.Modules.Events.Domain.Abstractions;
using Lerevently.Modules.Events.Domain.Events;

namespace Lerevently.Modules.Events.Application.Events.CancelEvent;

internal sealed class CancelEventCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IEventRepository eventRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CancelEventCommand>
{
    public async Task<Result> Handle(CancelEventCommand request, CancellationToken cancellationToken)
    {
        Event? @event = await eventRepository.GetAsync(request.EventId, cancellationToken);

        if (@event is null)
        {
            return Result.Failure(EventErrors.NotFound(request.EventId));
        }

        Result result = @event.Cancel(dateTimeProvider.UtcNow);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
