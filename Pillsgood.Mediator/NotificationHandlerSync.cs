namespace Pillsgood.Mediator;

/// <summary>
/// Wrapper class for a synchronous notification handler
/// </summary>
/// <typeparam name="TNotification">The notification type</typeparam>
public abstract class NotificationHandlerSync<TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{
    Task INotificationHandler<TNotification>.Handle(TNotification notification, CancellationToken cancellationToken)
    {
        Handle(notification);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Override in a derived class for the handler logic
    /// </summary>
    /// <param name="notification">Notification</param>
    protected abstract void Handle(TNotification notification);
}