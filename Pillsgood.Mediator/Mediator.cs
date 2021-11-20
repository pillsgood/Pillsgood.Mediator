using System.Runtime.InteropServices;
using Pillsgood.Mediator.Internal;
using Pillsgood.Mediator.Publishers;

namespace Pillsgood.Mediator;

/// <summary>
/// Default mediator implementation relying on single- and multi instance delegates for resolving handlers.
/// </summary>
public class Mediator : IMediator
{
    private readonly ISender _sender;
    private readonly IPublisher _publisher;

    public Mediator(ISender sender, IPublisher publisher)
    {
        _sender = sender;
        _publisher = publisher;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// Optional implementations fallback to default implementations when null.
    /// </summary>
    /// <param name="serviceFactory">The single instance factory.</param>
    /// <param name="sender">optional ISender implementation</param>
    /// <param name="publisher">optional IPublisher implementation</param>
    public Mediator(ServiceFactory serviceFactory, [Optional] ISender? sender, [Optional] IPublisher? publisher)
    {
        _sender = sender ?? new Sender(serviceFactory);
        _publisher = publisher ?? new PublisherBase(serviceFactory);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// Optional implementations fallback to default implementations when null.
    /// </summary>
    /// <param name="serviceFactory">The single instance factory.</param>
    public Mediator(ServiceFactory serviceFactory)
    {
        _sender = new Sender(serviceFactory);
        _publisher = new PublisherBase(serviceFactory);
    }

    /// <inheritdoc />
    public Task<TResponse> Send<TResponse>(ISignal<TResponse> signal, CancellationToken cancellationToken = default)
    {
        return _sender.Send(signal, cancellationToken);
    }

    /// <inheritdoc />
    public Task<object?> Send(object signal, CancellationToken cancellationToken = default)
    {
        return _sender.Send(signal, cancellationToken);
    }

    /// <inheritdoc />
    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        return _publisher.Publish(notification, cancellationToken);
    }

    /// <inheritdoc />
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        return _publisher.Publish(notification, cancellationToken);
    }
}