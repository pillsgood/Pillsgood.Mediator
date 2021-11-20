namespace Pillsgood.Mediator;

/// <summary>
/// Send a signal through the mediator pipeline to be handled by a single handler.
/// </summary>
public interface ISender
{
    /// <summary>
    /// Asynchronously send a signal to a single handler
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="signal">Signal object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
    Task<TResponse> Send<TResponse>(ISignal<TResponse> signal, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously send an object signal to a single handler via dynamic dispatch
    /// </summary>
    /// <param name="signal">Signal object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the send operation. The task result contains the type erased handler response</returns>
    Task<object?> Send(object signal, CancellationToken cancellationToken = default);
}