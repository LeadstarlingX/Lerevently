using Lerevently.Modules.Events.Domain.Abstractions;

namespace Lerevently.Modules.Events.Application.Abstractions.Exceptions;

public sealed class EventlyException : Exception
{
    public EventlyException(string requestName, Error? error = default, Exception? innerException = default)
        : base("Application exception", innerException)
    {
        RequestName = requestName;
        Error = error;
    }

    public string RequestName { get; }

    public Error? Error { get; }
}
