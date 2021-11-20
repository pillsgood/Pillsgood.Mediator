using System.Reactive;

namespace Pillsgood.Mediator;

public abstract class SignalHandlerSync<TSignal, TResponse> : ISignalHandler<TSignal, TResponse>
    where TSignal : ISignal<TResponse>
{
    Task<TResponse> ISignalHandler<TSignal, TResponse>.Handle(TSignal signal, CancellationToken cancellationToken)
    {
        return Task.FromResult(Handle(signal));
    }

    protected abstract TResponse Handle(TSignal signal);
}

public abstract class SignalHandlerSync<TSignal> : ISignalHandler<TSignal> where TSignal : ISignal<Unit>
{
    Task<Unit> ISignalHandler<TSignal, Unit>.Handle(TSignal signal, CancellationToken cancellationToken)
    {
        Handle(signal);
        return Task.FromResult(Unit.Default);
    }

    protected abstract void Handle(TSignal signal);
}