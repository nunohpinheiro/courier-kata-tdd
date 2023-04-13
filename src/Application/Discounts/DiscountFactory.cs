namespace CourierKata.Application.Discounts;

using CourierKata.Domain.Models;

internal static class DiscountFactory
{
    internal static IReadOnlyList<Discount> CreateDiscounts(IList<Parcel> parcels)
    {
        if (parcels?.Any() is not true)
            return Enumerable.Empty<Discount>().ToList().AsReadOnly();

        var smallParcelDiscounts = new SmallParcelManiaStrategy().GetDiscounts(parcels);
        var mediumParcelDiscounts = new MediumParcelManiaStrategy().GetDiscounts(parcels);
        var mixedParcelDiscounts = new MixedParcelManiaStrategy().GetDiscounts(parcels);

        return new List<Discount>()
            .AddRangeIfFilled(smallParcelDiscounts)
            .AddRangeIfFilled(mediumParcelDiscounts)
            .AddRangeIfFilled(mixedParcelDiscounts)
            .GetMaxDiscountsPerParcel();
    }

    private static List<Discount> AddRangeIfFilled(this List<Discount> discounts, IReadOnlyList<Discount> newDiscounts)
    {
        if (newDiscounts.Any())
            discounts.AddRange(newDiscounts);
        return discounts;
    }

    private static List<Discount> GetMaxDiscountsPerParcel(this List<Discount> allDiscounts)
        => allDiscounts
        .OrderByDescending(discount => discount.Value)
        .GroupBy(discount => discount.ParcelId) // Each parcel can only be used in a discount once
        .Select(discountGroup =>
            discountGroup.MaxBy(discount => discount.Value)!) // Combination of discounts that saves the most money must be used
        .ToList();
}
