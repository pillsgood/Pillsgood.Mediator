namespace Pillsgood.Mediator;

public static class ServiceFactoryExtensions
{
    public static T? GetInstance<T>(this ServiceFactory factory)
    {
        return (T?) factory(typeof(T));
    }

    public static IEnumerable<T>? GetInstances<T>(this ServiceFactory factory)
    {
        return (IEnumerable<T>?) factory(typeof(IEnumerable<T>));
    }
}