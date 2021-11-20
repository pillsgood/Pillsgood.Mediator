namespace Pillsgood.Mediator.Pipeline;

/// <summary>
/// Wrapper class that synchronously handles an exception from signal
/// </summary>
/// <typeparam name="TSignal">Signal type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
/// <typeparam name="TException">Exception type</typeparam>
public abstract class SignalExceptionHandlerSync<TSignal, TResponse, TException>
    : ISignalExceptionHandler<TSignal, TResponse, TException>
    where TSignal : ISignal<TResponse>
    where TException : Exception
{
    Task ISignalExceptionHandler<TSignal, TResponse, TException>.Handle(
        TSignal signal,
        TException exception,
        SignalExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        Handle(signal, exception, state);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Override in a derived class for the handler logic
    /// </summary>
    /// <param name="signal">Failed signal</param>
    /// <param name="exception">The thrown exception</param>
    /// <param name="state">The current state of handling the exception</param>
    protected abstract void Handle(
        TSignal signal,
        TException exception,
        SignalExceptionHandlerState<TResponse> state);
}

/// <summary>
/// Wrapper class that synchronously handles a base exception from signal
/// </summary>
/// <typeparam name="TSignal">Signal type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public abstract class SignalExceptionHandlerSync<TSignal, TResponse>
    : ISignalExceptionHandler<TSignal, TResponse, Exception>
    where TSignal : ISignal<TResponse>
{
    public Task Handle(
        TSignal signal,
        Exception exception,
        SignalExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken)
    {
        Handle(signal, exception, state);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Override in a derived class for the handler logic
    /// </summary>
    /// <param name="signal">Failed signal</param>
    /// <param name="exception">The thrown exception</param>
    /// <param name="state">The current state of handling the exception</param>
    protected abstract void Handle(
        TSignal signal,
        Exception exception,
        SignalExceptionHandlerState<TResponse> state);
}