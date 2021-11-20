using System.Reactive;

namespace Pillsgood.Mediator;

public static class UnitExtensions
{
    /// <summary>
    /// Returns a Unit Task.
    /// </summary>
    /// <param name="unit">Unit</param>
    /// <returns>Task of Unit</returns>
    public static Task<Unit> AsTask(this Unit unit)
    {
        return Task.FromResult(unit);
    }
}