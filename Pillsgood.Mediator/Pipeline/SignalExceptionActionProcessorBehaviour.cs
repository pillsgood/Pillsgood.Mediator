using System.Reflection;
using System.Runtime.ExceptionServices;
using Pillsgood.Mediator.Internal;

namespace Pillsgood.Mediator.Pipeline;

public class SignalExceptionActionProcessorBehaviour<TSignal, TResponse> : IPipelineBehaviour<TSignal, TResponse>
    where TSignal : ISignal<TResponse>
{
    private readonly ServiceFactory _serviceFactory;

    public SignalExceptionActionProcessorBehaviour(ServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }


    public async Task<TResponse> Handle(
        TSignal signal,
        HandleSignal<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            for (var exceptionType = exception.GetType();
                 exceptionType != typeof(object) && exceptionType != null;
                 exceptionType = exceptionType.BaseType)
            {
                var actionsForException = GetActionsForException(exceptionType, signal, out var actionMethod);
                foreach (var actionForException in actionsForException)
                {
                    try
                    {
                        if (actionMethod.Invoke(actionForException,
                                new object[] { signal, exception, cancellationToken }) is Task task)
                        {
                            await task.ConfigureAwait(false);
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                $"Could not create task for action method {actionMethod}.");
                        }
                    }
                    catch (TargetInvocationException invocationException)
                        when (invocationException.InnerException != null)
                    {
                        ExceptionDispatchInfo.Capture(invocationException.InnerException).Throw();
                    }
                }
            }

            throw;
        }
    }

    private IList<object> GetActionsForException(
        Type exceptionType,
        TSignal signal,
        out MethodInfo actionMethodInfo)
    {
        var exceptionActionInterfaceType =
            typeof(ISignalExceptionAction<,>).MakeGenericType(typeof(TSignal), exceptionType);
        var enumerableExceptionActionInterfaceType =
            typeof(IEnumerable<>).MakeGenericType(exceptionActionInterfaceType);
        actionMethodInfo =
            exceptionActionInterfaceType.GetMethod(nameof(ISignalExceptionAction<TSignal, Exception>.Execute))
            ?? throw new InvalidOperationException(
                $"Could not find method {nameof(ISignalExceptionAction<TSignal, Exception>.Execute)} on type {exceptionActionInterfaceType}");

        var actionsForException = (IEnumerable<object>) _serviceFactory(enumerableExceptionActionInterfaceType);

        return HandlersOrderer.Prioritize(actionsForException.ToList(), signal);
    }
}