using CodeMechanic.Types;

public readonly record struct IslandEvents(int Id, string Name) : IEnumeration<IslandEvents>
{
    public static readonly IslandEvents Load     = new(0, "load");
    public static readonly IslandEvents Revealed = new(1, "revealed");
    public static readonly IslandEvents Intersect = new(2, "intersect once");

    public static IEnumerable<IslandEvents> GetAll() => new[] { Load, Revealed, Intersect };

    public static IslandEvents FromValue(int value)
        => GetAll().FirstOrDefault(e => e.Id == value);

    public static IslandEvents FromName(string name)
        => GetAll().FirstOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}
