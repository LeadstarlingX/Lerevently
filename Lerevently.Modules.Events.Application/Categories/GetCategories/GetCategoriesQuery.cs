using Lerevently.Common.Application.Messaging;
using Lerevently.Modules.Events.Application.Categories.GetCategory;

namespace Lerevently.Modules.Events.Application.Categories.GetCategories;

public sealed record GetCategoriesQuery : IQuery<IReadOnlyCollection<CategoryResponse>>;