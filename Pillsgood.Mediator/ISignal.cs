using System.Reactive;

namespace Pillsgood.Mediator;

/// <summary>
/// Marker interface to represent a signal with a void response
/// </summary>
public interface ISignal : ISignal<Unit>
{
}

/// <summary>
/// Marker interface to represent a signal with a response
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
public interface ISignal<out TResponse> : IBaseSignal
{
}