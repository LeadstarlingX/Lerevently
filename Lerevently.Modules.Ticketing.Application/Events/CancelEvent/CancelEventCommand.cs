using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Ticketing.Application.Events.CancelEvent;

public sealed record CancelEventCommand(Guid EventId) : ICommand;
