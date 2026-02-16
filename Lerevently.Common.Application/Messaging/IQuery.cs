using Lerevently.Common.Domain.Abstractions;
using MediatR;

namespace Lerevently.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;