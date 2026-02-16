using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Events.Application.Categories.UpdateCategory;

public sealed record UpdateCategoryCommand(Guid CategoryId, string Name) : ICommand;