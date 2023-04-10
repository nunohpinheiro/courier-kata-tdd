namespace CourierKata.Application.OrderCost;

public record CalculateOrderCostCommand
{
    public IEnumerable<ParcelRequest> Parcels { get; init; } = Enumerable.Empty<ParcelRequest>();
}

public record ParcelRequest
{
    public decimal Length { get; init; }
    public decimal Width { get; init; }
    public decimal Height { get; init; }
}
