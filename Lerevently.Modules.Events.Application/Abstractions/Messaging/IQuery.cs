using Lerevently.Modules.Events.Domain.Abstractions;
using MediatR;

namespace Lerevently.Modules.Events.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
