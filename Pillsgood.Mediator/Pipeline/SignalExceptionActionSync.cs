namespace Pillsgood.Mediator.Pipeline;

/// <summary>
/// Wrapper class that synchronously performs an action on a signal for base exception
/// </summary>
/// <typeparam name="TSignal">Signal type</typeparam>
public abstract class SignalExceptionActionSync<TSignal> : ISignalExceptionAction<TSignal>
    where TSignal : IBaseSignal
{
    Task ISignalExceptionAction<TSignal, Exception>.Execute(
        TSignal signal,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Execute(signal, exception);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Override in a derived class for the action logic
    /// </summary>
    /// <param name="signal">Failed signal</param>
    /// <param name="exception">Original exception from signal handler</param>
    protected abstract void Execute(TSignal signal, Exception exception);
}

/// <summary>
/// Wrapper class that synchronously performs an action on a signal for specific exception
/// </summary>
/// <typeparam name="TSignal">Signal type</typeparam>
/// <typeparam name="TException">Exception type</typeparam>
public abstract class SignalExceptionActionSync<TSignal, TException> : ISignalExceptionAction<TSignal, TException>
    where TSignal : IBaseSignal
    where TException : Exception
{
    Task ISignalExceptionAction<TSignal, TException>.Execute(
        TSignal signal,
        TException exception,
        CancellationToken cancellationToken)
    {
        Execute(signal, exception);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Override in a derived class for the action logic
    /// </summary>
    /// <param name="signal">Failed signal</param>
    /// <param name="exception">Original exception from signal handler</param>
    protected abstract void Execute(TSignal signal, TException exception);
}