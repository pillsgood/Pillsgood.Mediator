namespace Pillsgood.Mediator.Internal
{
    internal class ObjectDetails : IComparer<ObjectDetails>
    {
        public string Name { get; }

        public string? AssemblyName { get; }

        public string? Location { get; }

        public object Value { get; }

        public Type Type { get; }

        public bool IsOverridden { get; set; }

        public ObjectDetails(object value)
        {
            Value = value;
            Type = Value.GetType();
            var exceptionHandlerType = value.GetType();

            Name = exceptionHandlerType.Name;
            AssemblyName = exceptionHandlerType.Assembly.GetName().Name;
            Location = exceptionHandlerType.Namespace?.Replace($"{AssemblyName}.", string.Empty);
        }

        public int Compare(ObjectDetails? x, ObjectDetails? y)
        {
            if (x == null)
            {
                return 1;
            }

            if (y == null)
            {
                return -1;
            }

            return CompareByAssembly(x, y) ?? CompareByNamespace(x, y) ?? CompareByLocation(x, y);
        }
        
        private int? CompareByAssembly(ObjectDetails x, ObjectDetails y)
        {
            if (x.AssemblyName == AssemblyName && y.AssemblyName != AssemblyName)
            {
                return -1;
            }

            if (x.AssemblyName != AssemblyName && y.AssemblyName == AssemblyName)
            {
                return 1;
            }

            if (x.AssemblyName != AssemblyName && y.AssemblyName != AssemblyName)
            {
                return 0;
            }

            return null;
        }

        private int? CompareByNamespace(ObjectDetails x, ObjectDetails y)
        {
            if (Location is null || x.Location is null || y.Location is null)
            {
                return 0;
            }

            if (x.Location.StartsWith(Location, StringComparison.Ordinal) &&
                !y.Location.StartsWith(Location, StringComparison.Ordinal))
            {
                return -1;
            }

            if (!x.Location.StartsWith(Location, StringComparison.Ordinal) &&
                y.Location.StartsWith(Location, StringComparison.Ordinal))
            {
                return 1;
            }

            if (x.Location.StartsWith(Location, StringComparison.Ordinal) &&
                y.Location.StartsWith(Location, StringComparison.Ordinal))
            {
                return 0;
            }

            return null;
        }


        private int CompareByLocation(ObjectDetails x, ObjectDetails y)
        {
            if (Location is null || x.Location is null || y.Location is null)
            {
                return 0;
            }

            if (Location.StartsWith(x.Location, StringComparison.Ordinal) &&
                !Location.StartsWith(y.Location, StringComparison.Ordinal))
            {
                return -1;
            }

            if (!Location.StartsWith(x.Location, StringComparison.Ordinal) &&
                Location.StartsWith(y.Location, StringComparison.Ordinal))
            {
                return 1;
            }

            if (x.Location.Length > y.Location.Length)
            {
                return -1;
            }

            if (x.Location.Length < y.Location.Length)
            {
                return 1;
            }

            return 0;
        }
    }
}