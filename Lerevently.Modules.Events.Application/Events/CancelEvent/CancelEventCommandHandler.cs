using Lerevently.Common.Application.Clock;
using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Events.Application.Abstractions.Data;
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
        var @event = await eventRepository.GetAsync(request.EventId, cancellationToken);

        if (@event is null) return Result.Failure(EventErrors.NotFound(request.EventId));

        var result = @event.Cancel(dateTimeProvider.UtcNow);

        if (result.IsFailure) return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}