using Pillsgood.Mediator.Wrappers;

namespace Pillsgood.Mediator.Publishers;

public class AsyncContinueOnExceptionPublisher : PublisherBase
{
    /// <inheritdoc />
    public AsyncContinueOnExceptionPublisher(ServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    /// <inheritdoc />
    protected override async Task PublishCore(
        IEnumerable<HandleNotification> handlers,
        INotification notification,
        CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>();
        var exceptions = new List<Exception>();

        foreach (var handler in handlers)
        {
            try
            {
                tasks.Add(handler(notification, cancellationToken));
            }
            catch (Exception ex) when (ex is not (OutOfMemoryException or StackOverflowException))
            {
                exceptions.Add(ex);
            }
        }

        try
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch (AggregateException ex)
        {
            exceptions.AddRange(ex.Flatten().InnerExceptions);
        }
        catch (Exception ex) when (ex is not (OutOfMemoryException or StackOverflowException))
        {
            exceptions.Add(ex);
        }

        if (exceptions.Any())
        {
            throw new AggregateException(exceptions);
        }
    }
}