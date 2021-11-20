using System.Reactive;

namespace Pillsgood.Mediator;

public static class UnitExtensions
{
    public static Task<Unit> AsTask(this Unit unit)
    {
        return Task.FromResult(unit);
    }
}