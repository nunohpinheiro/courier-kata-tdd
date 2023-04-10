namespace CourierKata.Application.UnitTests.Fixtures;

using CourierKata.Application.OrderCost;
using CourierKata.Domain.Models;

internal static class ParcelRequestFixtureFactory
{
    internal static ParcelRequest Create(ParcelSize parcelSize = default)
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
        };
    }

    internal static ParcelRequest CreateInvalid()
    {
        var fixture = new Fixture();
        fixture.Customize<ParcelRequest>(comp => comp
            .With(p => p.Length, GetNegativeDecimal)
            .With(p => p.Width, GetNegativeDecimal)
            .With(p => p.Height, GetNegativeDecimal));
        return fixture.Create<ParcelRequest>();
    }

    internal static IEnumerable<ParcelRequest> CreateMany(
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
