namespace CourierKata.Application.UnitTests.Fixtures;

using CourierKata.Application.OrderCost;

internal class CalculateOrderCostCommandAllInvalidParcelsAttribute : AutoDataAttribute
{
    public CalculateOrderCostCommandAllInvalidParcelsAttribute() : base(() =>
    {
        var fixture = new Fixture();
        fixture.Customize<ParcelRequest>(comp => comp
            .With(p => p.Length, GetNegativeDecimal)
            .With(p => p.Width, GetNegativeDecimal)
            .With(p => p.Height, GetNegativeDecimal)
            .With(p => p.Weight, (int)GetNegativeDecimal()));
        fixture.Customize<CalculateOrderCostCommand>(comp =>
            comp.With(c => c.Parcels, () => fixture.CreateMany<ParcelRequest>(new Random().Next(1, 10))));
        return fixture;
    })
    { }

    private static decimal GetNegativeDecimal()
        => new Random().Next(int.MinValue, 1);
}
