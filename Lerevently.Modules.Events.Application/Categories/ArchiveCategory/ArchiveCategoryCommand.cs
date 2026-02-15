using Lerevently.Modules.Events.Application.Abstractions.Messaging;

namespace Lerevently.Modules.Events.Application.Categories.ArchiveCategory;

public sealed record ArchiveCategoryCommand(Guid CategoryId) : ICommand;
