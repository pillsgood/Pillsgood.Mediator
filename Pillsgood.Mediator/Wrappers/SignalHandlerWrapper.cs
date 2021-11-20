namespace Pillsgood.Mediator.Wrappers;

public abstract class SignalHandlerWrapper<TResponse> : SignalHandlerBase
{
    public abstract Task<TResponse> Handle(
        ISignal<TResponse> signal,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken);
}

public class SignalHandlerWrapperImpl<TSignal, TResponse> : SignalHandlerWrapper<TResponse>
    where TSignal : ISignal<TResponse>
{
    public override async Task<object?> Handle(
        object signal,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        return await Handle((ISignal<TResponse>) signal, serviceFactory, cancellationToken)
            .ConfigureAwait(false);
    }

    public override Task<TResponse> Handle(
        ISignal<TResponse> signal,
        ServiceFactory serviceFactory,
        CancellationToken cancellationToken)
    {
        Task<TResponse> Handler() => GetHandler<ISignalHandler<TSignal, TResponse>>(serviceFactory)
            .Handle((TSignal) signal, cancellationToken);

        return serviceFactory
            .GetInstances<IPipelineBehaviour<TSignal, TResponse>>()
            .Reverse()
            .Aggregate((HandleSignal<TResponse>) Handler, (next, pipeline) =>
                () => pipeline.Handle((TSignal) signal, next, cancellationToken))
            .Invoke();
    }
}