using Lerevently.Modules.Events.Application.Abstractions.Messaging;
using MediatR;

namespace Lerevently.Modules.Events.Application.Events.CreateEvent;

public sealed record CreateEventCommand(
    Guid CategoryId,
    string Title,
    string Description,
    string Location,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc) : ICommand<Guid>;
