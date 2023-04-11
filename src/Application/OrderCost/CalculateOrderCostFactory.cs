namespace CourierKata.Application.OrderCost;

using CourierKata.Domain.Models;

internal static class CalculateOrderCostFactory
{
    internal static CalculateOrderCostResponse ToOrderCostResponse(this Order order)
    {
        if (order.Parcels?.Any() is not true)
            return new() { SpeedyShipping = order.SpeedyShipping };

        var parcelsCost = order.Parcels.Sum(p => p.Cost);
        var speedyShippingCost = order.SpeedyShipping ? parcelsCost : 0;
        var totalCost = parcelsCost + speedyShippingCost;

        return new()
        {
            Parcels = order.Parcels
                .Select(p => new ParcelResponse
                {
                    ParcelCost = p.Cost,
                    ParcelSize = p.Size.ToString(),
                    IsOverWeight = p.IsOverWeight
                }).ToList()
                .AsReadOnly(),
            SpeedyShipping = order.SpeedyShipping,
            SpeedyShippingCost = speedyShippingCost,
            TotalCost = totalCost
        };
    }

    internal static Order ToOrder(this CalculateOrderCostCommand calculateOrderCostCommand)
        => new(
            calculateOrderCostCommand.ToParcels(),
            calculateOrderCostCommand?.SpeedyShipping);

    private static List<Parcel> ToParcels(this CalculateOrderCostCommand calculateOrderCostCommand)
        => calculateOrderCostCommand?.Parcels.Any() is not true
        ? Enumerable.Empty<Parcel>().ToList()
        : calculateOrderCostCommand.Parcels
            .Select(p => new Parcel(p.Length, p.Width, p.Height, p.Weight))
            .ToList();
}
