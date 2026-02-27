using Lerevently.Common.Application.EventBus;
using Lerevently.Common.Application.Exceptions;
using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Users.Application.Users.GetUser;
using Lerevently.Modules.Users.Domain.Users;
using Lerevently.Modules.Users.IntegrationEvents;
using MediatR;

namespace Lerevently.Modules.Users.Application.Users.RegisterUser;

internal sealed class UserRegisteredDomainEventHandler(ISender sender, IEventBus eventBus)
    : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
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