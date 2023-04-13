namespace Application.UnitTests.OrderCost;

using CourierKata.Application.OrderCost;
using CourierKata.Application.UnitTests.Fixtures;
using CourierKata.Domain.Models;
using CourierKata.Tests.Common.Fixtures;
using CSharpFunctionalExtensions;

public class CalculateOrderCostFactoryTests
{
    [Theory, MemberData(nameof(CommandWithDefaultValues))]
    public void ToOrder_CommandHasDefaultValues_ReturnsEmpty(CalculateOrderCostCommand command)
        => command.ToOrder()
        .Should().Match<Order>(o =>
            !o.Parcels.Any()
            && !o.SpeedyShipping);

    public static IEnumerable<object[]> CommandWithDefaultValues =>
        new List<object[]>
        {
            new object[] { null! },
            new object[] { new CalculateOrderCostCommand() },
        };

    [Theory]
    [InlineData(true), InlineData(false)]
    public void ToOrder_CommandHasSeveralParcels_ReturnsSameAmountOfParcels(bool speedyShipping)
    {
        // Arrange
        Fixture fixture = new();

        var parcelsRequest1 = ParcelRequestFixtureFactory.CreateMany(
            fixture.Create<ParcelSize>(), new Random().Next(1, 5));
        var parcelsRequest2 = ParcelRequestFixtureFactory.CreateMany(
            fixture.Create<ParcelSize>(), new Random().Next(1, 5));

        List<ParcelRequest> parcelRequests = new();
        parcelRequests.AddRange(parcelsRequest1);
        parcelRequests.AddRange(parcelsRequest2);

        CalculateOrderCostCommand command = new()
        {
            Parcels = parcelRequests,
            SpeedyShipping = speedyShipping
        };

        // Act, Assert
        command.ToOrder()
            .Should().Match<Order>(o =>
                o.SpeedyShipping == speedyShipping
                && o.Parcels.Count == parcelRequests.Count);
    }

    [Theory, MemberData(nameof(OrderParcelsWithDefaultValues))]
    public void ToOrderCostResponse_ParcelsHaveDefaultValues_ReturnsDefaultProperties(Order order)
        => order.ToOrderCostResponse()
        .Should().Match<CalculateOrderCostResponse>(r =>
            r.Parcels.Count == 0
            && r.SpeedyShipping == order.SpeedyShipping
            && r.TotalCost == 0
            && r.TotalDiscount == 0
            && r.Discounts.Count == 0);

    public static IEnumerable<object[]> OrderParcelsWithDefaultValues =>
        new List<object[]>
        {
            new object[] { new Order(null!, new Fixture().Create<bool>()) },
            new object[] { new Order(new List<Parcel>(), new Fixture().Create<bool>()) },
        };

    [Fact]
    public void ToOrderCostResponse_ParcelsAreValid_NoOverWeight_NoDiscounts_ReturnsMatchingResponse()
    {
        // Arrange
        Fixture fixture = new();
        var parcelSize = fixture.Create<ParcelSize>();

        var parcels = ParcelFixtureFactory
            .CreateMany(parcelSize, new Random().Next(1, 5))
            .ToList();

        Order order = new(parcels, fixture.Create<bool>());

        // Act
        var orderCostResponse = order.ToOrderCostResponse();

        // Assert
        var parcelsCost = parcels.Count * parcels.First().Cost;
        var totalCost = order.SpeedyShipping ? parcelsCost * 2 : parcelsCost;

        orderCostResponse.Should().Match<CalculateOrderCostResponse>(r =>
            r.SpeedyShipping == order.SpeedyShipping
            && r.TotalCost == totalCost
            && r.Parcels.Count == parcels.Count);

        orderCostResponse.Parcels
            .Where(p => p.ParcelSize.Equals(parcelSize.ToString()) && p.ParcelCost == parcels.First().Cost)
            .Should().HaveCount(parcels.Count);
    }

    [Theory]
    [InlineData(true), InlineData(false)]
    public void ToOrderCostResponse_ParcelsAreValid_WithOverWeight_ReturnsMatchingResponse(bool heavyParcel)
    {
        // Arrange
        Fixture fixture = new();
        var parcelSize = ParcelSize.Small;
        var parcelWeight = new Random().Next(51, 1000); // Higher underweight limit is 50kg

        var parcels = ParcelFixtureFactory
            .CreateMany(parcelSize, new Random().Next(1, 5), parcelWeight, heavyParcel)
            .ToList();

        Order order = new(parcels, fixture.Create<bool>());

        // Act
        var orderCostResponse = order.ToOrderCostResponse();

        // Assert
        var parcelsCost = parcels.Count * parcels.First().Cost;
        var totalCost = order.SpeedyShipping ? parcelsCost * 2 : parcelsCost;

        orderCostResponse.Should().Match<CalculateOrderCostResponse>(r =>
            r.SpeedyShipping == order.SpeedyShipping
            && r.TotalCost == totalCost
            && r.Parcels.Count == parcels.Count);

        orderCostResponse.Parcels
            .Where(p =>
                p.ParcelSize.Equals(parcelSize.ToString())
                && p.ParcelCost == parcels.First().Cost
                && p.IsOverWeight
                && p.HeavyParcel == heavyParcel)
            .Should().HaveCount(parcels.Count);
    }

    [Fact]
    public void ToOrderCostResponse_ParcelsAreValid_WithDiscounts_ReturnsMatchingResponse()
    {
        // Arrange
        Fixture fixture = new();
        var parcelSize = fixture.Create<ParcelSize>();

        var parcels = ParcelFixtureFactory
            .CreateMany(parcelSize, new Random().Next(1, 5))
            .ToList();

        Order order = new(parcels, fixture.Create<bool>());

        var discounts = DiscountFixtureFactory.CreateMany(new Random().Next(1, 5)).ToList();
        order.SetDiscounts(discounts);

        // Act
        var orderCostResponse = order.ToOrderCostResponse();

        // Assert
        orderCostResponse.Should().Match<CalculateOrderCostResponse>(r =>
            r.SpeedyShipping == order.SpeedyShipping
            && r.TotalCost == GetExpectedCost(parcels, discounts, order.SpeedyShipping)
            && r.TotalDiscount == (-1 * discounts.Sum(d => d.Value))
            && r.Discounts.Count == discounts.Count
            && r.Parcels.Count == parcels.Count);
    }

    private static int GetExpectedCost(
        IEnumerable<Parcel> parcels, IEnumerable<Discount> discounts, bool speedyShipping)
    {
        var baseCost = parcels.Sum(p => p.Cost) - discounts.Sum(d => d.Value);
        return speedyShipping
            ? baseCost * 2
            : baseCost;
    }
}
