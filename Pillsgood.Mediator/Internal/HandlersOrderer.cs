namespace Pillsgood.Mediator.Internal;

internal static class HandlersOrderer
{
    public static IList<object> Prioritize<TSignal>(IList<object> handlers, TSignal signal)
        where TSignal : notnull
    {
        if (handlers.Count < 2)
        {
            return handlers;
        }

        var signalObjectDetails = new ObjectDetails(signal);
        var handlerObjectsDetails = handlers.Select(s => new ObjectDetails(s)).ToList();

        var uniqueHandlers = RemoveOverridden(handlerObjectsDetails).ToArray();
        Array.Sort(uniqueHandlers, signalObjectDetails);

        return uniqueHandlers.Select(s => s.Value).ToList();
    }

    private static IEnumerable<ObjectDetails> RemoveOverridden(IList<ObjectDetails> handlersData)
    {
        for (int i = 0; i < handlersData.Count - 1; i++)
        {
            for (int j = i + 1; j < handlersData.Count; j++)
            {
                if (handlersData[i].IsOverridden || handlersData[j].IsOverridden)
                {
                    continue;
                }

                if (handlersData[i].Type.IsAssignableFrom(handlersData[j].Type))
                {
                    handlersData[i].IsOverridden = true;
                }
                else if (handlersData[j].Type.IsAssignableFrom(handlersData[i].Type))
                {
                    handlersData[j].IsOverridden = true;
                }
            }
        }

        return handlersData.Where(w => !w.IsOverridden);
    }
}