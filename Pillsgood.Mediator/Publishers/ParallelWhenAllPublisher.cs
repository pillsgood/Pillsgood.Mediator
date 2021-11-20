using Pillsgood.Mediator.Wrappers;

namespace Pillsgood.Mediator.Publishers;

public class ParallelWhenAllPublisher : PublisherBase
{
    /// <inheritdoc />
    public ParallelWhenAllPublisher(ServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    /// <inheritdoc />
    protected override Task PublishCore(
        IEnumerable<HandleNotification> handlers,
        INotification notification,
        CancellationToken cancellationToken = default)
    {
        var tasks = handlers
            .Select(handler =>
                Task.Run(() => handler(notification, cancellationToken), cancellationToken))
            .ToList();
        return Task.WhenAll(tasks);
    }
}