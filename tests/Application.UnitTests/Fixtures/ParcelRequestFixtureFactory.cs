namespace CourierKata.Application.UnitTests.Fixtures;

using CourierKata.Application.OrderCost;
using CourierKata.Domain.Models;

internal static class ParcelRequestFixtureFactory
{
    internal static ParcelRequest Create(
        ParcelSize parcelSize = default, int weight = 1, bool heavyParcel = false)
    {
        var minSize = parcelSize switch
        {
            ParcelSize.Small => 1,
            ParcelSize.Medium => 10,
            ParcelSize.Large => 50,
            _ => 100
        };
        var maxSize = parcelSize switch
        {
            ParcelSize.Small => 10,
            ParcelSize.Medium => 50,
            ParcelSize.Large => 100,
            _ => int.MaxValue
        };
        return new()
        {
            Length = new Random().Next(minSize, maxSize),
            Width = new Random().Next(minSize, maxSize),
            Height = new Random().Next(minSize, maxSize),
            Weight = weight,
            HeavyParcel = heavyParcel
        };
    }

    internal static ParcelRequest CreateInvalid()
    {
        var fixture = new Fixture();
        fixture.Customize<ParcelRequest>(comp => comp
            .With(p => p.Length, GetNonPositiveDecimal)
            .With(p => p.Width, GetNonPositiveDecimal)
            .With(p => p.Height, GetNonPositiveDecimal)
            .With(p => p.Weight, (int)GetNonPositiveDecimal()));
        return fixture.Create<ParcelRequest>();
    }

    internal static IEnumerable<ParcelRequest> CreateMany(
        ParcelSize parcelSize = default, int parcelsCount = 2, int weight = 1, bool heavyParcel = false)
    {
        for (int i = 0; i < parcelsCount; i++)
        {
            yield return Create(parcelSize, weight, heavyParcel);
        }
    }

    private static decimal GetNonPositiveDecimal()
        => new Random().Next(int.MinValue, 1);
}
