namespace Pillsgood.Mediator.Pipeline;

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

    protected abstract void Handle(
        TSignal signal,
        TException exception,
        SignalExceptionHandlerState<TResponse> state);
}

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

    protected abstract void Handle(
        TSignal signal,
        Exception exception,
        SignalExceptionHandlerState<TResponse> state);
}