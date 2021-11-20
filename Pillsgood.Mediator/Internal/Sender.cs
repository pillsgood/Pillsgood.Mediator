using System.Collections.Concurrent;
using Pillsgood.Mediator.Wrappers;

namespace Pillsgood.Mediator.Internal;

internal class Sender : ISender
{
    private static readonly ConcurrentDictionary<Type, SignalHandlerBase> _signalHandlers = new();
    private readonly ServiceFactory _serviceFactory;

    public Sender(ServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }


    public Task<TResponse> Send<TResponse>(ISignal<TResponse> signal, CancellationToken cancellationToken = default)
    {
        if (signal is null)
        {
            throw new ArgumentNullException(nameof(signal));
        }

        var signalType = signal.GetType();
        var handler = (SignalHandlerWrapper<TResponse>) _signalHandlers.GetOrAdd(signalType,
            static t => Activator.CreateInstance(
                    typeof(SignalHandlerWrapperImpl<,>).MakeGenericType(t, typeof(TResponse)))
                is SignalHandlerBase handlerBase
                ? handlerBase
                : throw new InvalidOperationException($"Could not create wrapper type for {t}"));

        return handler.Handle(signal, _serviceFactory, cancellationToken);
    }

    public Task<object?> Send(object signal, CancellationToken cancellationToken = default)
    {
        if (signal is null)
        {
            throw new ArgumentNullException(nameof(signal));
        }

        var signalType = signal.GetType();
        var handler = _signalHandlers.GetOrAdd(signalType,
            static t =>
            {
                var signalInterfaceType = t.GetInterfaces()
                    .FirstOrDefault(static i => i.IsGenericType && i
                        .GetGenericTypeDefinition() == typeof(ISignal<>));
                if (signalInterfaceType is null)
                {
                    throw new ArgumentException($"{t.Name} does not implement {nameof(ISignal)}", nameof(signal));
                }

                var responseType = signalInterfaceType.GetGenericArguments()[0];
                var wrapperType = typeof(SignalHandlerWrapperImpl<,>).MakeGenericType(t, responseType);
                return Activator.CreateInstance(wrapperType) is SignalHandlerBase handlerBase
                    ? handlerBase
                    : throw new InvalidOperationException($"Could not create wrapper for type {wrapperType}");
            });

        return handler.Handle(signal, _serviceFactory, cancellationToken);
    }
}