namespace Pillsgood.Mediator;

public abstract class NotificationHandlerSync<TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{
    Task INotificationHandler<TNotification>.Handle(TNotification notification, CancellationToken cancellationToken)
    {
        Handle(notification);
        return Task.CompletedTask;
    }

    protected abstract void Handle(TNotification notification);
}