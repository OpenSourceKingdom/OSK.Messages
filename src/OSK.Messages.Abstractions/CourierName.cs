namespace OSK.Messages.Abstractions;

/// <summary>
/// A strongly-typed name for a given courier. i.e. this represents the name of a courier integration into the message system.
/// </summary>
/// <param name="Name">The name of the courier</param>
public readonly record struct CourierName(string Name)
{
    public static implicit operator CourierName(string name) => new(name);
}
