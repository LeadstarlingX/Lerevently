using Lerevently.Modules.Events.Application.Abstractions.Messaging;
using Lerevently.Modules.Events.Application.TicketTypes.GetTicketType;

namespace Lerevently.Modules.Events.Application.TicketTypes.GetTicketTypes;

public sealed record GetTicketTypesQuery(Guid EventId) : IQuery<IReadOnlyCollection<TicketTypeResponse>>;
