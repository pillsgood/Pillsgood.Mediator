namespace Pillsgood.Mediator.Pipeline;

public interface ISignalExceptionAction<in TSignal, in TException>
    where TSignal : IBaseSignal
    where TException : Exception
{
    Task Execute(TSignal signal, TException exception, CancellationToken cancellationToken);
}

public interface ISignalExceptionAction<in TSignal> : ISignalExceptionAction<TSignal, Exception>
    where TSignal : IBaseSignal
{
}