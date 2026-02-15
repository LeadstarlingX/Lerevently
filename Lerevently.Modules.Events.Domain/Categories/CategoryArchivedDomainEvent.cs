using Lerevently.Modules.Events.Domain.Abstractions;

namespace Lerevently.Modules.Events.Domain.Categories;

public sealed class CategoryArchivedDomainEvent(Guid categoryId) : DomainEvent
{
    public Guid CategoryId { get; init; } = categoryId;
}
