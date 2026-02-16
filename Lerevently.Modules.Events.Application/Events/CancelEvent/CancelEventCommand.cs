using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Events.Application.Events.CancelEvent;

public sealed record CancelEventCommand(Guid EventId) : ICommand;