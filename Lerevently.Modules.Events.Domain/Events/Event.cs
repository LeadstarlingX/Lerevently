namespace Lerevently.Modules.Events.Domain.Events
{
    public sealed class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public DateTime StartsAtUtc { get; set; }
        public DateTime? EndsAtUtc { get; set; }
        public EventStatus Status { get; set; }
    }

}
