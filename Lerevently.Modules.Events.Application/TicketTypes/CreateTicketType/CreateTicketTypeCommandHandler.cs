using Lerevently.Common.Application.Messaging;
using Lerevently.Common.Domain.Abstractions;
using Lerevently.Modules.Events.Application.Abstractions.Data;
using Lerevently.Modules.Events.Domain.Events;
using Lerevently.Modules.Events.Domain.TicketTypes;

namespace Lerevently.Modules.Events.Application.TicketTypes.CreateTicketType;

internal sealed class CreateTicketTypeCommandHandler(
    IEventRepository eventRepository,
    ITicketTypeRepository ticketTypeRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateTicketTypeCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetAsync(request.EventId, cancellationToken);

        if (@event is null) return Result.Failure<Guid>(EventErrors.NotFound(request.EventId));

        var ticketType = TicketType.Create(@event, request.Name, request.Price, request.Currency, request.Quantity);

        ticketTypeRepository.Insert(ticketType);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ticketType.Id;
    }
}