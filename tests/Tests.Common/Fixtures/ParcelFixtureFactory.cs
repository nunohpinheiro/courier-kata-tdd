namespace CourierKata.Tests.Common.Fixtures;

using AutoFixture;
using CourierKata.Domain.Models;
using CourierKata.Domain.ValueObjects;

public static class ParcelFixtureFactory
{
    public static Parcel Create(ParcelSize parcelSize = default)
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
        return new(
            length: new Random().Next(minSize, maxSize),
            width: new Random().Next(minSize, maxSize),
            height: new Random().Next(minSize, maxSize));
    }

    public static Parcel CreateInvalid()
    {
        var fixture = new Fixture();
        fixture.Customize<Parcel>(comp => comp
            .With(p => p.Length, PositiveDecimal.From(GetNegativeDecimal()))
            .With(p => p.Width, PositiveDecimal.From(GetNegativeDecimal()))
            .With(p => p.Height, PositiveDecimal.From(GetNegativeDecimal())));
        return fixture.Create<Parcel>();
    }

    public static IEnumerable<Parcel> CreateMany(
        ParcelSize parcelSize = default, int parcelsCount = 2)
    {
        for (int i = 0; i < parcelsCount; i++)
        {
            yield return Create(parcelSize);
        }
    }

    private static decimal GetNegativeDecimal()
        => new Random().Next(int.MinValue, 1);
}
