namespace Pillsgood.Mediator.Pipeline;

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

    protected abstract void Execute(TSignal signal, Exception exception);
}

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

    protected abstract void Execute(TSignal signal, TException exception);
}