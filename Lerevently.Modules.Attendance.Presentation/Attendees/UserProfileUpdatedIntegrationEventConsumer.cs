using Lerevently.Common.Application.Exceptions;
using Lerevently.Modules.Attendance.Application.Attendees.UpdateAttendee;
using Lerevently.Modules.Users.IntegrationEvents;
using MassTransit;
using MediatR;

namespace Lerevently.Modules.Attendance.Presentation.Attendees;

public sealed class UserProfileUpdatedIntegrationEventConsumer(ISender sender)
    : IConsumer<UserProfileUpdatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserProfileUpdatedIntegrationEvent> context)
    {
        var result = await sender.Send(
            new UpdateAttendeeCommand(
                context.Message.UserId,
                context.Message.FirstName,
                context.Message.LastName),
            context.CancellationToken);

        if (result.IsFailure) throw new EventlyException(nameof(UpdateAttendeeCommand), result.Error);
    }
}