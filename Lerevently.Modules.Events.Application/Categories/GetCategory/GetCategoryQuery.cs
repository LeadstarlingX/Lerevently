using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Events.Application.Categories.GetCategory;

public sealed record GetCategoryQuery(Guid CategoryId) : IQuery<CategoryResponse>;