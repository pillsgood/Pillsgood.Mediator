namespace Pillsgood.Mediator.Pipeline;

/// <summary>
/// Defines a signal post-processor for a signal
/// </summary>
/// <typeparam name="TSignal">Signal type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public interface ISignalPostProcessor<in TSignal, in TResponse> where TSignal : ISignal<TResponse>
{
    /// <summary>
    /// Process method executes after the Handle method on your handler
    /// </summary>
    /// <param name="signal">Signal instance</param>
    /// <param name="response">Response instance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An awaitable task</returns>
    Task PostProcess(TSignal signal, TResponse response, CancellationToken cancellationToken);
}