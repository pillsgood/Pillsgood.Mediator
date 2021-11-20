namespace Pillsgood.Mediator;

/// <summary>
/// Pipeline behavior to surround the inner handler.
/// Implementations add additional behavior and await the next delegate.
/// </summary>
/// <typeparam name="TSignal">Signal type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IPipelineBehaviour<in TSignal, TResponse> where TSignal : ISignal<TResponse>
{
    
    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the <paramref name="next"/> delegate as necessary
    /// </summary>
    /// <param name="signal">Incoming signal</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    /// <returns>Awaitable task returning the <typeparamref name="TResponse"/></returns>
    Task<TResponse> Handle(
        TSignal signal,
        HandleSignal<TResponse> next,
        CancellationToken cancellationToken);
}