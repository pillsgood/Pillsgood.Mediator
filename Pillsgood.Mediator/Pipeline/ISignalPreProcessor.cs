namespace Pillsgood.Mediator.Pipeline;

/// <summary>
/// Defined a signal pre-processor for a handler
/// </summary>
/// <typeparam name="TSignal">Signal type</typeparam>
public interface ISignalPreProcessor<in TSignal> where TSignal : IBaseSignal
{
    /// <summary>
    /// Process method executes before calling the Handle method on your handler
    /// </summary>
    /// <param name="signal">Incoming signal</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An awaitable task</returns>
    Task PreProcess(TSignal signal, CancellationToken cancellationToken);
}