namespace CourierKata.Application.OrderCost;

public record CalculateOrderCostCommand
{
    public IEnumerable<ParcelRequest> Parcels { get; init; } = Enumerable.Empty<ParcelRequest>();
    public bool SpeedyShipping { get; init; }
}

public record ParcelRequest
{
    public decimal Length { get; init; }
    public decimal Width { get; init; }
    public decimal Height { get; init; }
    public int Weight { get; init; }
    public bool HeavyParcel { get; init; }
}
