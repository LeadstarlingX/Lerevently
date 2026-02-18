using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Users.Application.Users.GetUser;

public sealed record GetUserQuery(Guid UserId) : IQuery<UserResponse>;
