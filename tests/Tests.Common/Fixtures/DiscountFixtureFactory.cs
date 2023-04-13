namespace CourierKata.Tests.Common.Fixtures;

using AutoFixture;
using CourierKata.Domain.Models;

public static class DiscountFixtureFactory
{
    public static Discount Create()
        => new Fixture().Create<Discount>();

    public static IEnumerable<Discount> CreateMany(int discountsCount = 2)
    {
        for (int i = 0; i < discountsCount; i++)
        {
            yield return Create();
        }
    }
}
