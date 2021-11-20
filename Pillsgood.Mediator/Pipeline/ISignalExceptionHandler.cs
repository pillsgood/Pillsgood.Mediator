namespace Pillsgood.Mediator.Pipeline;

public interface ISignalExceptionHandler<in TSignal, TResponse, in TException>
    where TSignal : ISignal<TResponse>
    where TException : Exception
{
    Task Handle(
        TSignal signal,
        TException exception,
        SignalExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken);
}

public interface ISignalExceptionHandler<in TSignal, TResponse> : ISignalExceptionHandler<TSignal, TResponse, Exception>
    where TSignal : ISignal<TResponse>
{
}