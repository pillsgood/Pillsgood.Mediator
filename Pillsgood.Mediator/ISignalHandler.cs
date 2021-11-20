using System.Reactive;

namespace Pillsgood.Mediator;

public interface ISignalHandler<in TSignal, TResponse>
    where TSignal : ISignal<TResponse>
{
    Task<TResponse> Handle(TSignal signal, CancellationToken cancellationToken);
}

public interface ISignalHandler<in TSignal> : ISignalHandler<TSignal, Unit> where TSignal : ISignal<Unit>
{
}