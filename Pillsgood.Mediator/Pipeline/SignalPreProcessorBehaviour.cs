namespace Pillsgood.Mediator.Pipeline;

/// <summary>
/// Behavior for executing all <see cref="ISignalPreProcessor{TSignal}"/> instances before handling a signal
/// </summary>
/// <typeparam name="TSignal">Signal type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public class SignalPreProcessorBehaviour<TSignal, TResponse> : IPipelineBehaviour<TSignal, TResponse>
    where TSignal : ISignal<TResponse>
{
    private readonly IEnumerable<ISignalPreProcessor<TSignal>> _preProcessors;


    public SignalPreProcessorBehaviour(IEnumerable<ISignalPreProcessor<TSignal>> preProcessors)
    {
        _preProcessors = preProcessors;
    }

    public async Task<TResponse> Handle(
        TSignal signal,
        HandleSignal<TResponse> next,
        CancellationToken cancellationToken)
    {
        foreach (var processor in _preProcessors)
        {
            await processor.PreProcess(signal, cancellationToken).ConfigureAwait(false);
        }

        return await next().ConfigureAwait(false);
    }
}