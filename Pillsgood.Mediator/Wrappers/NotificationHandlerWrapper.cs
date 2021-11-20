namespace Pillsgood.Mediator.Wrappers;

public delegate Task HandleNotification(
    INotification notification,
    CancellationToken cancellationToken);

public delegate Task PublishNotification(
    IEnumerable<HandleNotification> handlers,
    INotification notification,
    CancellationToken cancellationToken);

public abstract class NotificationHandlerWrapper
{
    public abstract Task Handle(
        INotification notification,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken,
        PublishNotification publishNotification);
}

public class NotificationHandlerWrapperImpl<TNotification> : NotificationHandlerWrapper
    where TNotification : INotification
{
    public override Task Handle(
        INotification notification,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken,
        PublishNotification publishNotification)
    {
        var handlers = serviceFactory
            .GetInstances<INotificationHandler<TNotification>>()
            .Select(handler => new HandleNotification((n, token) => handler.Handle((TNotification) n, token)));

        return publishNotification(handlers, notification, cancellationToken);
    }
}