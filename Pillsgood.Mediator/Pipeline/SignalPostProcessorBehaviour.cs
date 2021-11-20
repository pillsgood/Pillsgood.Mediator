namespace Pillsgood.Mediator.Pipeline;

/// <summary>
/// Behavior for executing all <see cref="ISignalPostProcessor{TSignal,TResponse}"/> instances after handling the signal
/// </summary>
/// <typeparam name="TSignal">Signal type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public class SignalPostProcessorBehaviour<TSignal, TResponse> : IPipelineBehaviour<TSignal, TResponse>
    where TSignal : ISignal<TResponse>
{
    private readonly IEnumerable<ISignalPostProcessor<TSignal, TResponse>> _postProcessors;


    public SignalPostProcessorBehaviour(IEnumerable<ISignalPostProcessor<TSignal, TResponse>> postProcessors)
    {
        _postProcessors = postProcessors;
    }

    public async Task<TResponse> Handle(
        TSignal signal,
        HandleSignal<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next().ConfigureAwait(false);
        foreach (var processor in _postProcessors)
        {
            await processor.PostProcess(signal, response, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }
}