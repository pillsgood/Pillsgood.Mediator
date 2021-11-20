namespace Pillsgood.Mediator.Pipeline;

/// <summary>
/// Defines an exception action for a signal
/// </summary>
/// <typeparam name="TSignal">Signal type</typeparam>
/// <typeparam name="TException">Exception type</typeparam>
public interface ISignalExceptionAction<in TSignal, in TException>
    where TSignal : IBaseSignal
    where TException : Exception
{
    /// <summary>
    /// Called when the signal handler throws an exception
    /// </summary>
    /// <param name="signal">Signal instance</param>
    /// <param name="exception">The thrown exception</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An awaitable task</returns>
    Task Execute(TSignal signal, TException exception, CancellationToken cancellationToken);
}

/// <summary>
/// Defines the base exception action for a signal.
///     You do not need to register this interface explicitly
///     with a container as it inherits from the base
///     <see cref="ISignalExceptionAction{TSignal, TException}" /> interface.
/// </summary>
/// <typeparam name="TSignal">The type of failed signal</typeparam>
public interface ISignalExceptionAction<in TSignal> : ISignalExceptionAction<TSignal, Exception>
    where TSignal : IBaseSignal
{
}