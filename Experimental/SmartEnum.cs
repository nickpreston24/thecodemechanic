namespace CodeMechanic.Types
{
    /// <summary>
    /// Value-type SmartEnum (record struct) with implicit string/int conversions.
    /// Single file — no extra dependencies.
    /// </summary>
    public readonly record struct SmartEnum(int Id, string Name) : IComparable<SmartEnum>, IEquatable<SmartEnum>
    {
        public override string ToString() => Name;

        // Implicit conversions (your favorite feature)
        // public static implicit operator SmartEnum(string name) => FromName(name);
        // public static implicit operator SmartEnum(int id)     => FromValue(id);

        public static implicit operator string(SmartEnum e) => e.Name;
        public static implicit operator int(SmartEnum e) => e.Id;

        public int CompareTo(SmartEnum other) => Id.CompareTo(other.Id);

        public bool Equals(SmartEnum other) => Id == other.Id;

        // public override bool Equals(object? obj) => obj is SmartEnum other && Equals(other);
        public override int GetHashCode() => Id.GetHashCode();

        // Static lookup methods
        public static T FromValue<T>(int value) where T : struct, IEnumeration<T>
            => T.FromValue(value);

        public static T FromName<T>(string name) where T : struct, IEnumeration<T>
            => T.FromName(name);

        public static IEnumerable<T> GetAll<T>() where T : struct, IEnumeration<T>
            => T.GetAll();
    }

    /// <summary>
    /// Interface that concrete SmartEnum types must implement
    /// </summary>
    public interface IEnumeration<T> where T : struct, IEnumeration<T>
    {
        static abstract T FromValue(int value);
        static abstract T FromName(string name);
        static abstract IEnumerable<T> GetAll();
    }
}
