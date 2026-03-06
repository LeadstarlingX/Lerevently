using MassTransit;

namespace Lerevently.Modules.Events.Presentation.Events.CancelEventSaga;

public sealed class CancelEventState : SagaStateMachineInstance, ISagaVersion
{
    public string CurrentState { get; set; }

    public int CancellationCompletedStatus { get; set; }

    public int Version { get; set; }
    public Guid CorrelationId { get; set; }
}