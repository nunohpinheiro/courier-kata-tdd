namespace CourierKata.Domain.Models;

using CourierKata.Domain.Errors;
using CourierKata.Domain.Types;
using CSharpFunctionalExtensions;

public record Order
{
    private const int SpeedyShippingFraction = 2;

    public IReadOnlyList<Parcel> Parcels { get; init; } = Enumerable.Empty<Parcel>().ToList();
    public bool SpeedyShipping { get; init; }

    public IReadOnlyList<Discount> Discounts { get; private set; } = Enumerable.Empty<Discount>().ToList();

    public int TotalDiscount => Discounts.Sum(discount => discount.Value) * -1;

    public int TotalCost
    {
        get
        {
            var baseCost = Parcels.Sum(p => p.Cost) + TotalDiscount;
            return SpeedyShipping
                ? baseCost * SpeedyShippingFraction
                : baseCost;
        }
    }

    public Order(IEnumerable<Parcel>? parcels, bool? speedyShipping = null)
    {
        Parcels = (parcels ?? Enumerable.Empty<Parcel>()).ToList();
        SpeedyShipping = speedyShipping ?? false;
    }

    public Result<Success, Error> SetDiscounts(IReadOnlyList<Discount> discounts)
    {
        if (Discounts.Any())
            return new OrderParcelsCannotBeOverwrittenError();

        Discounts = discounts?.Any() is true
            ? discounts
            : Enumerable.Empty<Discount>().ToList();

        return new Success();
    }

    public Result<Success, Error> Validate()
        => Parcel.Validate(Parcels);
}
