using Lerevently.Modules.Events.Application.Abstractions.Messaging;

namespace Lerevently.Modules.Events.Application.Categories.CreateCategory;

public sealed record CreateCategoryCommand(string Name) : ICommand<Guid>;
