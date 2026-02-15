using Lerevently.Modules.Events.Application.Abstractions.Messaging;

namespace Lerevently.Modules.Events.Application.Events.RescheduleEvent;

public sealed record RescheduleEventCommand(Guid EventId, DateTime StartsAtUtc, DateTime? EndsAtUtc) : ICommand;
