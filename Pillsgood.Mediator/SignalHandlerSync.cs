using System.Reactive;

namespace Pillsgood.Mediator;


/// <summary>
/// Wrapper class for a handler that synchronously handles a signal and returns a response
/// </summary>
/// <typeparam name="TSignal">The type of signal being handled</typeparam>
/// <typeparam name="TResponse">The type of response from the handler</typeparam>
public abstract class SignalHandlerSync<TSignal, TResponse> : ISignalHandler<TSignal, TResponse>
    where TSignal : ISignal<TResponse>
{
    Task<TResponse> ISignalHandler<TSignal, TResponse>.Handle(TSignal signal, CancellationToken cancellationToken)
    {
        return Task.FromResult(Handle(signal));
    }

    /// <summary>
    /// Override in a derived class for the handler logic
    /// </summary>
    /// <param name="signal">Signal</param>
    /// <returns>Response</returns>
    protected abstract TResponse Handle(TSignal signal);
}

/// <summary>
/// Wrapper class for a handler that synchronously handles a signal does not return a response
/// </summary>
/// <typeparam name="TSignal">The type of signal being handled</typeparam>
public abstract class SignalHandlerSync<TSignal> : ISignalHandler<TSignal> where TSignal : ISignal<Unit>
{
    Task<Unit> ISignalHandler<TSignal, Unit>.Handle(TSignal signal, CancellationToken cancellationToken)
    {
        Handle(signal);
        return Task.FromResult(Unit.Default);
    }

    /// <summary>
    /// Override in a derived class for the handler logic
    /// </summary>
    /// <param name="signal">Signal</param>
    protected abstract void Handle(TSignal signal);
}