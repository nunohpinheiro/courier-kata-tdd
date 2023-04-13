namespace CourierKata.Domain.Models;

using System;

public record Discount(
    Guid ParcelId,
    int Value,
    string Description)
{
    public override string ToString()
        => $"{Description} - Discount value: {Value}";
}
