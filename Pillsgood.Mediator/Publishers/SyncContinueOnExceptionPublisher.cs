using Pillsgood.Mediator.Wrappers;

namespace Pillsgood.Mediator.Publishers;

public class SyncContinueOnExceptionPublisher : PublisherBase
{
    /// <inheritdoc />
    public SyncContinueOnExceptionPublisher(ServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    /// <inheritdoc />
    protected override async Task PublishCore(
        IEnumerable<HandleNotification> handlers,
        INotification notification,
        CancellationToken cancellationToken = default)
    {
        var exception = new List<Exception>();
        foreach (var handler in handlers)
        {
            try
            {
                await handler(notification, cancellationToken).ConfigureAwait(false);
            }
            catch (AggregateException e)
            {
                exception.AddRange(e.Flatten().InnerExceptions);
            }
            catch (Exception e) when (e is not (OutOfMemoryException or StackOverflowException))
            {
                exception.Add(e);
            }
        }

        if (exception.Any())
        {
            throw new AggregateException(exception);
        }
    }
}