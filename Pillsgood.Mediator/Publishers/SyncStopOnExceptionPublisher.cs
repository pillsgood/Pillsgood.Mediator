using Pillsgood.Mediator.Wrappers;

namespace Pillsgood.Mediator.Publishers;

public class SyncStopOnExceptionPublisher : PublisherBase
{
    /// <inheritdoc />
    public SyncStopOnExceptionPublisher(ServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    /// <inheritdoc />
    protected override async Task PublishCore(
        IEnumerable<HandleNotification> handlers,
        INotification notification,
        CancellationToken cancellationToken = default)
    {
        foreach (var handler in handlers)
        {
            await handler(notification, cancellationToken).ConfigureAwait(false);
        }
    }
}