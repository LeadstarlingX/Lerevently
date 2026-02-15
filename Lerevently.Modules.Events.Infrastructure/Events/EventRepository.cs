using Lerevently.Modules.Events.Domain.Events;
using Lerevently.Modules.Events.Infrastructure.Database;

namespace Lerevently.Modules.Events.Infrastructure.Events;

internal sealed class EventRepository(EventsDbContext context) : IEventRepository
{
    public void Insert(Event @event)
    {
        context.Events.Add(@event);
    }


}