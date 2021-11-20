namespace Pillsgood.Mediator.Publishers;

public class CompositePublisher : IPublisher
{
    private const PublishStrategy DefaultStrategy = PublishStrategy.SyncContinueOnException;
    private readonly IReadOnlyDictionary<PublishStrategy, IPublisher> _publishers;

    public CompositePublisher(ServiceFactory serviceFactory)
    {
        _publishers = new Dictionary<PublishStrategy, IPublisher>()
        {
            [PublishStrategy.SyncContinueOnException] = new SyncContinueOnExceptionPublisher(serviceFactory),
            [PublishStrategy.SyncStopOnException] = new SyncStopOnExceptionPublisher(serviceFactory),
            [PublishStrategy.AsyncContinueOnException] = new AsyncContinueOnExceptionPublisher(serviceFactory),
            [PublishStrategy.ParallelNoWait] = new ParallelNoWaitPublisher(serviceFactory),
            [PublishStrategy.ParallelWhenAll] = new ParallelWhenAllPublisher(serviceFactory),
            [PublishStrategy.ParallelWhenAny] = new ParallelWhenAnyPublisher(serviceFactory)
        };
    }

    /// <inheritdoc />
    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        return _publishers[DefaultStrategy].Publish(notification, cancellationToken);
    }

    /// <inheritdoc />
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        return _publishers[DefaultStrategy].Publish(notification, cancellationToken);
    }

    /// <inheritdoc />
    public Task Publish(object notification, PublishStrategy strategy, CancellationToken cancellationToken = default)
    {
        return _publishers[strategy].Publish(notification, cancellationToken);
    }

    /// <inheritdoc />
    public Task Publish<TNotification>(
        TNotification notification,
        PublishStrategy strategy,
        CancellationToken cancellationToken = default) where TNotification : INotification
    {
        return _publishers[strategy].Publish(notification, cancellationToken);
    }
}