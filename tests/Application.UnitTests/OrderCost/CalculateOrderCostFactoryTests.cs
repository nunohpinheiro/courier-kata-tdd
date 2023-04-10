namespace Application.UnitTests.OrderCost;

using CourierKata.Application.OrderCost;
using CourierKata.Application.UnitTests.Fixtures;
using CourierKata.Domain.Models;
using CourierKata.Tests.Common.Fixtures;
using CSharpFunctionalExtensions;

public class CalculateOrderCostFactoryTests
{
    [Theory, MemberData(nameof(ParcelsWithDefaultValues))]
    public void ToOrderCostResponse_ParcelsHaveDefaultValues_ReturnsDefaultProperties(List<Parcel> parcels)
        => parcels.ToOrderCostResponse()
        .Should().Match<CalculateOrderCostResponse>(r =>
            r.Parcels.Count == 0
            && r.TotalCost == 0);

    public static IEnumerable<object[]> ParcelsWithDefaultValues =>
        new List<object[]>
        {
            new object[] { null! },
            new object[] { new List<Parcel>() },
        };

    [Fact]
    public void ToOrderCostResponse_ParcelsAreValid_ReturnsMatchingResponse()
    {
        // Arrange
        Fixture fixture = new();
        var parcelSize = fixture.Create<ParcelSize>();

        var parcels = ParcelFixtureFactory
            .CreateMany(parcelSize, new Random().Next(1, 5))
            .ToList();

        // Act
        var orderCostResponse = parcels.ToOrderCostResponse();

        // Assert
        orderCostResponse.Should().Match<CalculateOrderCostResponse>(r =>
            r.TotalCost == parcels.Count * parcels.First().GetCost()
            && r.Parcels.Count == parcels.Count);

        orderCostResponse.Parcels
            .Where(p => p.ParcelSize.Equals(parcelSize.ToString()) && p.ParcelCost == parcels.First().GetCost())
            .Should().HaveCount(parcels.Count);
    }

    [Theory, MemberData(nameof(CommandWithDefaultValues))]
    public void ToParcels_CommandHasDefaultValues_ReturnsEmpty(CalculateOrderCostCommand command)
        => command.ToParcels()
        .Should().BeEmpty();

    public static IEnumerable<object[]> CommandWithDefaultValues =>
        new List<object[]>
        {
            new object[] { null! },
            new object[] { new CalculateOrderCostCommand() },
        };

    [Fact]
    public void ToParcels_CommandHasSeveralParcels_ReturnsSameAmountOfParcels()
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

        CalculateOrderCostCommand command = new() { Parcels = parcelRequests };

        // Act, Assert
        command.ToParcels()
            .Should().HaveCount(parcelRequests.Count);
    }
}
