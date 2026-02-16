using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Events.Application.Categories.ArchiveCategory;

public sealed record ArchiveCategoryCommand(Guid CategoryId) : ICommand;