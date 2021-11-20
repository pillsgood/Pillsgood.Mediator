using System.Reflection.Metadata;

namespace Pillsgood.Mediator.Wrappers;

public abstract class SignalHandlerBase
{
    public abstract Task<object?> Handle(
        object signal,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken);

    protected static THandler GetHandler<THandler>(ServiceFactory factory)
    {
        THandler handler;
        try
        {
            handler = factory.GetInstance<THandler>();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Error constructing handler for signal of type {typeof(THandler)}. " +
                                                "Register your handlers with the container.", e);
        }

        if (handler is null)
        {
            throw new InvalidOperationException($"Handler was not found for signal of type {typeof(THandler)}. " +
                                                $"Register your handlers with the container.");
        }

        return handler;
    }
}