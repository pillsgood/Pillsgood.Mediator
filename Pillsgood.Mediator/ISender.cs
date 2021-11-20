namespace Pillsgood.Mediator;

public interface ISender
{
    Task<TResponse> Send<TResponse>(ISignal<TResponse> signal, CancellationToken cancellationToken = default);
    
    Task<object?> Send(object signal, CancellationToken cancellationToken = default);
}