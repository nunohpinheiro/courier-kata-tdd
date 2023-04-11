namespace CourierKata.Domain.ValueObjects;

using CourierKata.Domain.Errors;

public class PositiveInteger : ValueResult<int, PositiveInteger>
{
    protected override void Validate()
        => Value = TryValidate()
        ? GetValue()
        : new ArgumentError($"{GetValue()} is not a positive integer.");

    protected override bool TryValidate()
        => GetValue() > 0;

    public static implicit operator PositiveInteger(int value)
        => From(value);

    public static implicit operator int(PositiveInteger result)
        => result.GetValue();
}
