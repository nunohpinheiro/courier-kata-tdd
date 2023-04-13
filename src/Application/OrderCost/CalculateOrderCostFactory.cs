namespace CourierKata.Application.OrderCost;

using CourierKata.Domain.Models;

internal static class CalculateOrderCostFactory
{
    internal static CalculateOrderCostResponse ToOrderCostResponse(this Order order)
        => new()
        {
            Parcels = order.Parcels
                .Select(p => new ParcelResponse
                {
                    ParcelCost = p.Cost,
                    ParcelSize = p.Size.ToString(),
                    IsOverWeight = p.IsOverWeight,
                    HeavyParcel = p.HeavyParcel
                }).ToList()
                .AsReadOnly(),
            SpeedyShipping = order.SpeedyShipping,
            Discounts = order.Discounts
                .Select(d => d.ToString())
                .ToList(),
            TotalDiscount = order.TotalDiscount,
            TotalCost = order.TotalCost
        };

    internal static Order ToOrder(this CalculateOrderCostCommand calculateOrderCostCommand)
        => new(
            calculateOrderCostCommand.ToParcels(),
            calculateOrderCostCommand?.SpeedyShipping);

    private static List<Parcel> ToParcels(this CalculateOrderCostCommand calculateOrderCostCommand)
        => calculateOrderCostCommand?.Parcels.Any() is true
        ? calculateOrderCostCommand.Parcels
            .Select(p => new Parcel(p.Length, p.Width, p.Height, p.Weight, p.HeavyParcel))
            .ToList()
        : Enumerable.Empty<Parcel>().ToList();
}
