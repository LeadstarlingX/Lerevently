using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Events.Application.Events.PublishEvent;

public sealed record PublishEventCommand(Guid EventId) : ICommand;