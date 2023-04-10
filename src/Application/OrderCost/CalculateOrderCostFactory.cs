namespace CourierKata.Application.OrderCost;

using CourierKata.Domain.Models;

internal static class CalculateOrderCostFactory
{
    internal static CalculateOrderCostResponse ToOrderCostResponse(this List<Parcel> parcels)
        => parcels?.Any() is not true
        ? new()
        : new()
        {
            Parcels = parcels
                .Select(p => new ParcelResponse
                {
                    ParcelCost = p.GetCost(),
                    ParcelSize = p.GetSize().ToString()
                }).ToList()
                .AsReadOnly(),
            TotalCost = parcels.Sum(p => p.GetCost())
        };

    internal static List<Parcel> ToParcels(this CalculateOrderCostCommand calculateOrderCostCommand)
        => calculateOrderCostCommand?.Parcels.Any() is not true
        ? Enumerable.Empty<Parcel>().ToList()
        : calculateOrderCostCommand.Parcels
            .Select(p => new Parcel(p.Length, p.Width, p.Height))
            .ToList();
}
