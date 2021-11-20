using System.Reactive;

namespace Pillsgood.Mediator;

public interface ISignal : ISignal<Unit>
{
}

public interface ISignal<out TResponse> : IBaseSignal
{
}