using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Events.Application.Events.GetEvent;

public sealed record GetEventQuery(Guid EventId) : IQuery<EventResponse?>;