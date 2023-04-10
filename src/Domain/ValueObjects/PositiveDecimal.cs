namespace CourierKata.Domain.ValueObjects;

using CourierKata.Domain.Errors;

public class PositiveDecimal : ValueResult<decimal, PositiveDecimal>
{
    protected override void Validate()
        => Value = TryValidate()
        ? GetValue()
        : new ArgumentError($"{GetValue()} is not a positive decimal.");

    protected override bool TryValidate()
        => GetValue() > 0;

    public static implicit operator PositiveDecimal(decimal value)
        => From(value);

    public static implicit operator decimal(PositiveDecimal result)
        => result.GetValue();
}
