namespace Pillsgood.Mediator.Pipeline;

public interface ISignalPostProcessor<in TSignal, in TResponse> where TSignal : ISignal<TResponse>
{
    Task PostProcess(TSignal signal, TResponse response, CancellationToken cancellationToken);
}