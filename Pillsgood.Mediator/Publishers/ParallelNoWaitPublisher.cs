using Pillsgood.Mediator.Wrappers;

namespace Pillsgood.Mediator.Publishers;

public class ParallelNoWaitPublisher : PublisherBase
{
    /// <inheritdoc />
    public ParallelNoWaitPublisher(ServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    /// <inheritdoc />
    protected override Task PublishCore(
        IEnumerable<HandleNotification> handlers,
        INotification notification,
        CancellationToken cancellationToken = default)
    {
        foreach (var handler in handlers)
        {
            Task.Run(() => handler(notification, cancellationToken), cancellationToken);
        }

        return Task.CompletedTask;
    }
}