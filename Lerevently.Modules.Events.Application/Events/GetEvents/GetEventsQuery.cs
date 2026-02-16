using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Events.Application.Events.GetEvents;

public sealed record GetEventsQuery : IQuery<IReadOnlyCollection<EventResponse>>;