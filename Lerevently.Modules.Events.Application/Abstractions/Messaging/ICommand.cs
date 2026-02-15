using Lerevently.Modules.Events.Domain.Abstractions;
using MediatR;

namespace Lerevently.Modules.Events.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

public interface IBaseCommand;
