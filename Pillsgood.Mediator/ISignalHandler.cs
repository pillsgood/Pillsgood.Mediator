using System.Reactive;

namespace Pillsgood.Mediator;

/// <summary>
/// Defines a handler for a signal
/// </summary>
/// <typeparam name="TSignal">The type of signal being handled</typeparam>
/// <typeparam name="TResponse">The type of response from the handler</typeparam>
public interface ISignalHandler<in TSignal, TResponse>
    where TSignal : ISignal<TResponse>
{
    /// <summary>
    /// Handles a signal
    /// </summary>
    /// <param name="signal">The signal</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the signal</returns>
    Task<TResponse> Handle(TSignal signal, CancellationToken cancellationToken);
}

/// <summary>
/// Defines a handler for a signal with a void (<see cref="Unit" />) response.
/// You do not need to register this interface explicitly with a container as it inherits from the base <see cref="ISignalHandler{TSignal, TResponse}" /> interface.
/// </summary>
/// <typeparam name="TSignal">The type of signal being handled</typeparam>
public interface ISignalHandler<in TSignal> : ISignalHandler<TSignal, Unit> where TSignal : ISignal<Unit>
{
}