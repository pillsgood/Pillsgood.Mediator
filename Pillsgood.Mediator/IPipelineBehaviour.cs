namespace Pillsgood.Mediator;

public delegate Task<TResponse> HandleSignal<TResponse>();

public interface IPipelineBehaviour<in TSignal, TResponse> where TSignal : ISignal<TResponse>
{
    Task<TResponse> Handle(
        TSignal signal,
        HandleSignal<TResponse> next,
        CancellationToken cancellationToken);
}