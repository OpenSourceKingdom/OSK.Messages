namespace OSK.Messages.Abstractions;

public readonly record struct CourierName(string Name)
{
    public static implicit operator CourierName(string name) => new CourierName(name);
}
