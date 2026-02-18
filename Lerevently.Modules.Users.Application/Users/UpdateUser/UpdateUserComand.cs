

using Lerevently.Common.Application.Messaging;

namespace Lerevently.Modules.Users.Application.Users.UpdateUser;

public sealed record UpdateUserCommand(Guid UserId, string FirstName, string LastName) : ICommand;
