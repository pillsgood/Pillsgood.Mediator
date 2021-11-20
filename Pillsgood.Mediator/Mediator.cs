using System.Collections.Concurrent;
using Pillsgood.Mediator.Wrappers;

namespace Pillsgood.Mediator;

/// <summary>
/// Default mediator implementation relying on single- and multi instance delegates for resolving handlers.
/// </summary>
public class Mediator : IMediator
{
    private readonly ServiceFactory _serviceFactory;
    
    private static readonly ConcurrentDictionary<Type, SignalHandlerBase> _signalHandlers = new();
    private static readonly ConcurrentDictionary<Type, NotificationHandlerWrapper> _notificationHandler = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceFactory">The single instance factory.</param>
    public Mediator(ServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    /// <summary>
    /// Override in a derived class to control how the tasks are awaited. By default the implementation is a foreach and await of each handler
    /// </summary>
    /// <param name="handlers">Enumerable of tasks representing invoking each notification handler</param>
    /// <param name="notification">The notification being published</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing invoking all handlers</returns>
    protected virtual async Task PublishCore(
        IEnumerable<HandleNotification> handlers,
        INotification notification,
        CancellationToken cancellationToken = default)
    {
        foreach (var handler in handlers)
        {
            await handler(notification, cancellationToken).ConfigureAwait(false);
        }
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

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (notification is null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        return PublishNotification(notification, cancellationToken);
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        return notification switch
        {
            null => throw new ArgumentNullException(nameof(notification)),
            INotification instance => PublishNotification(instance, cancellationToken),
            _ => throw new ArgumentException($"{nameof(notification)} does not implement {nameof(INotification)}")
        };
    }

    private Task PublishNotification(INotification notification, CancellationToken cancellationToken = default)
    {
        var notificationType = notification.GetType();
        var handler = _notificationHandler.GetOrAdd(notificationType,
            static t => Activator.CreateInstance(typeof(NotificationHandlerWrapperImpl<>).MakeGenericType(t))
                is NotificationHandlerWrapper handlerWrapper
                ? handlerWrapper
                : throw new InvalidOperationException($"Could not create wrapper for type {t}"));

        return handler.Handle(notification, _serviceFactory, cancellationToken, PublishCore);
    }
}