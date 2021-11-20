namespace Pillsgood.Mediator.Pipeline;

/// <summary>
/// Defines an exception handler for a signal and response
/// </summary>
/// <typeparam name="TSignal">Signal type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
/// <typeparam name="TException">Exception type</typeparam>
public interface ISignalExceptionHandler<in TSignal, TResponse, in TException>
    where TSignal : ISignal<TResponse>
    where TException : Exception
{
    /// <summary>
    /// Called when the signal handler throws an exception
    /// </summary>
    /// <param name="signal">Signal instance</param>
    /// <param name="exception">The thrown exception</param>
    /// <param name="state">The current state of handling the exception</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An awaitable task</returns>
    Task Handle(
        TSignal signal,
        TException exception,
        SignalExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken);
}

/// <summary>
/// Defines the base exception handler for a signal and response
/// </summary>
/// <typeparam name="TSignal">Signal type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public interface ISignalExceptionHandler<in TSignal, TResponse> : ISignalExceptionHandler<TSignal, TResponse, Exception>
    where TSignal : ISignal<TResponse>
{
}