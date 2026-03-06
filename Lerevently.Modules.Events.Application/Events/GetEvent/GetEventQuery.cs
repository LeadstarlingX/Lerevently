using Lerevently.Common.Application.Messaging;
using MediatR;

namespace Lerevently.Modules.Events.Application.Events.GetEvent;

public sealed record GetEventQuery(Guid EventId) : IQuery<EventResponse?>;