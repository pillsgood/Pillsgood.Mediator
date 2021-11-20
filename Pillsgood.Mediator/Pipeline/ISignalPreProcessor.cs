namespace Pillsgood.Mediator.Pipeline;

public interface ISignalPreProcessor<in TSignal> where TSignal : IBaseSignal
{
    Task PreProcess(TSignal signal, CancellationToken cancellationToken);
}