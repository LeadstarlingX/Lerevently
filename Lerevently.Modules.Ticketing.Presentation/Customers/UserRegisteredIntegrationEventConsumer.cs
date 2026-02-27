using Lerevently.Common.Application.Exceptions;
using Lerevently.Modules.Ticketing.Application.Customers.CreateCustomer;
using Lerevently.Modules.Users.IntegrationEvents;
using MassTransit;
using MediatR;

namespace Lerevently.Modules.Ticketing.Presentation.Customers;

public sealed class UserRegisteredIntegrationEventConsumer(ISender sender)
    : IConsumer<UserRegisteredIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredIntegrationEvent> context)
    {
        var result = await sender.Send(
            new CreateCustomerCommand(
                context.Message.UserId,
                context.Message.Email,
                context.Message.FirstName,
                context.Message.LastName),
            context.CancellationToken);

        if (result.IsFailure) throw new EventlyException(nameof(CreateCustomerCommand), result.Error);
    }
}