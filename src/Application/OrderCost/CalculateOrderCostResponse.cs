namespace CourierKata.Application.OrderCost;

public record CalculateOrderCostResponse
{
    public IReadOnlyCollection<ParcelResponse> Parcels { get; init; } = Enumerable.Empty<ParcelResponse>().ToList().AsReadOnly();
    public int TotalCost { get; init; }
}

public record ParcelResponse
{
    public int ParcelCost { get; init; }
    public string ParcelSize { get; init; } = string.Empty;
}
