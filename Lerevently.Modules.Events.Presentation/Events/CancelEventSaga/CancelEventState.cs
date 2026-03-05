using Lerevently.Modules.Events.Domain.Events;
using MassTransit;

namespace Lerevently.Modules.Events.Presentation.Events.CancelEventSaga;

public sealed class CancelEventState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    
    public int Version { get; set; }
    
    public string CurrentState { get; set; }
    
    public int CancellationCompletedStatus { get; set; }
}

