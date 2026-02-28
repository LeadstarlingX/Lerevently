using Lerevently.Common.Application.EventBus;
using Lerevently.Common.Application.Exceptions;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Users.Application.Users.GetUser;
using Lerevently.Modules.Users.Domain.Users;
using Lerevently.Modules.Users.IntegrationEvents;
using MediatR;

namespace Lerevently.Modules.Users.Application.Users.RegisterUser;

internal sealed class UserRegisteredDomainEventHandler(ISender sender, IEventBus eventBus)
    : DomainEventHandler<UserRegisteredDomainEvent>
{
    public override async Task Handle(UserRegisteredDomainEvent notification,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetUserQuery(notification.UserId), cancellationToken);

        if (result.IsFailure) throw new EventlyException(nameof(GetUserQuery), result.Error);

        await eventBus.PublishAsync(
            new UserRegisteredIntegrationEvent(
                notification.Id,
                notification.OccurredAtUtc,
                result.Value.Id,
                result.Value.Email,
                result.Value.FirstName,
                result.Value.LastName),
            cancellationToken
        );
    }
}