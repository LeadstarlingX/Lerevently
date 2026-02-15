using Lerevently.Modules.Events.Application.Abstractions.Clock;

namespace Lerevently.Modules.Events.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
