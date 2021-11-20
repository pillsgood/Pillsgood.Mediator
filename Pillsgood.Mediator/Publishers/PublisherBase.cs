using System.Collections.Concurrent;
using Pillsgood.Mediator.Wrappers;

namespace Pillsgood.Mediator.Publishers;

public class PublisherBase : IPublisher
{
    private static readonly ConcurrentDictionary<Type, NotificationHandlerWrapper> _notificationHandler = new();
    private readonly ServiceFactory _serviceFactory;

    public PublisherBase(ServiceFactory serviceFactory)
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

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (notification is null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        return Publish((INotification) notification, cancellationToken);
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        return notification switch
        {
            null => throw new ArgumentNullException(nameof(notification)),
            INotification instance => Publish(instance, cancellationToken),
            _ => throw new ArgumentException($"{nameof(notification)} does not implement {nameof(INotification)}")
        };
    }

    private Task Publish(INotification notification, CancellationToken cancellationToken = default)
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