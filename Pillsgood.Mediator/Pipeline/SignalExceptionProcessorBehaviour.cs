using System.Reflection;
using System.Runtime.ExceptionServices;
using Pillsgood.Mediator.Internal;

namespace Pillsgood.Mediator.Pipeline;

/// <summary>
/// Behavior for executing all <see cref="ISignalExceptionHandler{TSignal,TResponse,TException}"/>
///     or <see cref="ISignalExceptionHandler{TSignal,TResponse}"/> instances
///     after an exception is thrown by the following pipeline steps
/// </summary>
/// <typeparam name="TSignal">Signal type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public class SignalExceptionProcessorBehaviour<TSignal, TResponse> : IPipelineBehaviour<TSignal, TResponse>
    where TSignal : ISignal<TResponse>
{
    private readonly ServiceFactory _serviceFactory;

    public SignalExceptionProcessorBehaviour(ServiceFactory serviceFactory)
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
            var state = new SignalExceptionHandlerState<TResponse>();
            var exceptionType = (Type?) null;
            while (!state.Handled && exceptionType != typeof(Exception))
            {
                exceptionType = exceptionType == null
                    ? exception.GetType()
                    : exceptionType.BaseType
                      ?? throw new InvalidOperationException("Could not determine exception base type.");
                var exceptionHandlers = GetExceptionHandlers(signal, exceptionType, out var handleMethod);
                foreach (var exceptionHandler in exceptionHandlers)
                {
                    try
                    {
                        if (handleMethod.Invoke(exceptionHandler, new object?[]
                            {
                                signal, exception, state, cancellationToken
                            }) is Task task)
                        {
                            await task.ConfigureAwait(false);
                        }
                        else
                        {
                            throw new InvalidOperationException("Did not return a task from the exception handler.");
                        }
                    }
                    catch (TargetInvocationException invocationException)
                        when (invocationException.InnerException != null)
                    {
                        ExceptionDispatchInfo.Capture(invocationException.InnerException).Throw();
                    }

                    if (state.Handled)
                    {
                        break;
                    }
                }
            }

            if (!state.Handled || state.Response is null)
            {
                throw;
            }

            return state.Response;
        }
    }

    private IList<object> GetExceptionHandlers(
        TSignal signal,
        Type exceptionType,
        out MethodInfo handleMethodInfo)
    {
        var exceptionHandlerInterfaceType = typeof(ISignalExceptionHandler<,,>)
            .MakeGenericType(typeof(TSignal), typeof(TResponse), exceptionType);
        var enumerableExceptionHandlerInterfaceType = typeof(IEnumerable<>)
            .MakeGenericType(exceptionHandlerInterfaceType);
        handleMethodInfo = exceptionHandlerInterfaceType.GetMethod(
                               nameof(ISignalExceptionHandler<TSignal, TResponse, Exception>.Handle))
                           ?? throw new InvalidOperationException(
                               $"Could not find method {nameof(ISignalExceptionHandler<TSignal, TResponse, Exception>.Handle)} on type {exceptionHandlerInterfaceType}");

        var exceptionHandlers = (IEnumerable<object>) _serviceFactory.Invoke(enumerableExceptionHandlerInterfaceType);

        return HandlersOrderer.Prioritize(exceptionHandlers.ToList(), signal);
    }
}