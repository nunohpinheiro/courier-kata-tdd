namespace CourierKata.Application.OrderCost;

public record CalculateOrderCostResponse
{
    public IReadOnlyCollection<ParcelResponse> Parcels { get; init; } = Enumerable.Empty<ParcelResponse>().ToList().AsReadOnly();
    public bool SpeedyShipping { get; init; }
    public IReadOnlyCollection<string> Discounts { get; init; } = Enumerable.Empty<string>().ToList().AsReadOnly();
    public int TotalDiscount { get; init; }
    public int TotalCost { get; init; }
}

public record ParcelResponse
{
    public int ParcelCost { get; init; }
    public string ParcelSize { get; init; } = string.Empty;
    public bool IsOverWeight { get; init; }
    public bool HeavyParcel { get; init; }
}
