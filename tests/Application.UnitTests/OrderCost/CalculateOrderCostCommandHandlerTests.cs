namespace Application.UnitTests.OrderCost;

using CourierKata.Application.OrderCost;
using CourierKata.Application.UnitTests.Fixtures;
using CourierKata.Domain.Errors;
using CourierKata.Domain.Models;
using System.Reflection;

[UsesVerify]
public class CalculateOrderCostCommandHandlerTests
{
    private readonly string SnapshotFilesPath;

    public CalculateOrderCostCommandHandlerTests()
    {
        SnapshotFilesPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "snapshots");
    }

    [Theory, CalculateOrderCostCommandAllInvalidParcels]
    public void Handle_CommandHasAllInvalidParcels_ReturnsFailureWithArgumentError(CalculateOrderCostCommand command)
    {
        var result = CalculateOrderCostCommandHandler.Handle(command);

        result.Should().Match<Result<CalculateOrderCostResponse, Error>>(r =>
            r.IsFailure
            && r.Error is AggregateError
            && ((AggregateError)r.Error).InnerErrors.All(e => e is ArgumentError));
    }

    [Theory, MemberData(nameof(CommandWithDefaultValues))]
    public void Handle_CommandHasDefaultValues_ReturnsFailureWithParcelsCollectionIsEmptyError(CalculateOrderCostCommand command)
    {
        var result = CalculateOrderCostCommandHandler.Handle(command);

        result.Should().Match<Result<CalculateOrderCostResponse, Error>>(r =>
            r.IsFailure
            && r.Error is ParcelsCollectionIsEmptyError);
    }

    public static IEnumerable<object[]> CommandWithDefaultValues =>
        new List<object[]>
        {
            new object[] { null! },
            new object[] { new CalculateOrderCostCommand() },
        };

    [Fact]
    public void Handle_CommandHasInvalidAndValidParcels_ReturnsFailureWithArgumentError()
    {
        // Arrange
        CalculateOrderCostCommand command = new()
        {
            Parcels = new List<ParcelRequest>
            {
                ParcelRequestFixtureFactory.Create(),
                ParcelRequestFixtureFactory.CreateInvalid()
            }
        };

        // Act
        var result = CalculateOrderCostCommandHandler.Handle(command);

        // Assert
        result.Should().Match<Result<CalculateOrderCostResponse, Error>>(r =>
            r.IsFailure
            && r.Error is AggregateError
            && ((AggregateError)r.Error).InnerErrors.All(e => e is ArgumentError));
    }

    [Theory, MemberData(nameof(CommandWithParcelsOfSameDimension))]
    public void Handle_CommandHasParcelsOfSameDimension_ReturnsSuccessWithMatchingResponse(
        ParcelSize parcelSize, int parcelsCount, int expectedParcelPrice, bool speedyShipping)
    {
        // Arrange
        CalculateOrderCostCommand command = new()
        {
            Parcels = ParcelRequestFixtureFactory.CreateMany(parcelSize, parcelsCount),
            SpeedyShipping = speedyShipping
        };

        // Act
        var result = CalculateOrderCostCommandHandler.Handle(command);

        // Assert
        var parcelsCost = parcelsCount * expectedParcelPrice;
        var speedyShippingCost = command.SpeedyShipping ? parcelsCost : 0;
        var totalCost = parcelsCost + speedyShippingCost;

        result.Should().Match<Result<CalculateOrderCostResponse, Error>>(r =>
            r.IsSuccess
            && r.Value.SpeedyShipping == command.SpeedyShipping
            && r.Value.SpeedyShippingCost == speedyShippingCost
            && r.Value.TotalCost == totalCost
            && r.Value.Parcels.Count == command.Parcels.Count());

        AssertParcelsGroup(result, parcelSize, parcelsCount);
    }

    public static IEnumerable<object[]> CommandWithParcelsOfSameDimension =>
        new List<object[]>
        {
            new object[] { ParcelSize.Small, new Random().Next(1, 5), GetParcelPrice(ParcelSize.Small), new Fixture().Create<bool>() },
            new object[] { ParcelSize.Medium, new Random().Next(1, 5), GetParcelPrice(ParcelSize.Medium), new Fixture().Create<bool>() },
            new object[] { ParcelSize.Large, new Random().Next(1, 5), GetParcelPrice(ParcelSize.Large), new Fixture().Create<bool>() },
            new object[] { ParcelSize.ExtraLarge, new Random().Next(1, 5), GetParcelPrice(ParcelSize.ExtraLarge), new Fixture().Create<bool>() },
        };

    [Theory]
    [InlineData(true), InlineData(false)]
    public void Handle_CommandHasParcelsWithDifferentDimensions_ReturnsSuccessWithMatchingResponse(bool speedyShipping)
    {
        // Arrange
        (IEnumerable<ParcelRequest> smallParcels, int smallParcelsCount) = CreateParcelFixtures(ParcelSize.Small);
        (IEnumerable<ParcelRequest> mediumParcels, int mediumParcelsCount) = CreateParcelFixtures(ParcelSize.Medium);
        (IEnumerable<ParcelRequest> largeParcels, int largeParcelsCount) = CreateParcelFixtures(ParcelSize.Large);
        (IEnumerable<ParcelRequest> extraLargeParcels, int extraLargeParcelsCount) = CreateParcelFixtures(ParcelSize.ExtraLarge);

        var expectedParcelsCost = smallParcelsCount * GetParcelPrice(ParcelSize.Small)
            + mediumParcelsCount * GetParcelPrice(ParcelSize.Medium)
            + largeParcelsCount * GetParcelPrice(ParcelSize.Large)
            + extraLargeParcelsCount * GetParcelPrice(ParcelSize.ExtraLarge);
        var expectedSpeedyShippingCost = speedyShipping ? expectedParcelsCost : 0;
        var expectedTotalCost = expectedParcelsCost + expectedSpeedyShippingCost;

        List<ParcelRequest> parcelRequests = new();
        parcelRequests.AddRange(smallParcels);
        parcelRequests.AddRange(mediumParcels);
        parcelRequests.AddRange(largeParcels);
        parcelRequests.AddRange(extraLargeParcels);

        CalculateOrderCostCommand command = new()
        {
            Parcels = parcelRequests,
            SpeedyShipping = speedyShipping
        };

        // Act
        var result = CalculateOrderCostCommandHandler.Handle(command);

        // Assert
        result.Should().Match<Result<CalculateOrderCostResponse, Error>>(r =>
            r.IsSuccess
            && r.Value.SpeedyShipping == speedyShipping
            && r.Value.SpeedyShippingCost == expectedSpeedyShippingCost
            && r.Value.TotalCost == expectedTotalCost
            && r.Value.Parcels.Count == command.Parcels.Count());

        AssertParcelsGroup(result, ParcelSize.Small, smallParcelsCount);
        AssertParcelsGroup(result, ParcelSize.Medium, mediumParcelsCount);
        AssertParcelsGroup(result, ParcelSize.Large, largeParcelsCount);
        AssertParcelsGroup(result, ParcelSize.ExtraLarge, extraLargeParcelsCount);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task Handle_CommandHasParcelsWithFixedValues_ReturnsSuccessWithFixedSnapshot(
        bool speedyShipping, bool overWeight1kg)
    {
        // Arrange
        CalculateOrderCostCommand command = new()
        {
            Parcels = new List<ParcelRequest>(4)
            {
                new(){ Length = 1, Width = 2, Height = 3, Weight = overWeight1kg ? 2 : 1 },
                new(){ Length = 11, Width = 12, Height = 13, Weight = overWeight1kg ? 4 : 3 },
                new(){ Length = 51, Width = 52, Height = 53, Weight = overWeight1kg ? 7 : 6 },
                new(){ Length = 101, Width = 102, Height = 103, Weight = overWeight1kg ? 11 : 10 },
            },
            SpeedyShipping = speedyShipping
        };

        var expectedParcelsCost = GetParcelPrice(ParcelSize.Small, overWeight1kg)
            + GetParcelPrice(ParcelSize.Medium, overWeight1kg)
            + GetParcelPrice(ParcelSize.Large, overWeight1kg)
            + GetParcelPrice(ParcelSize.ExtraLarge, overWeight1kg);
        var expectedSpeedyShippingCost = speedyShipping ? expectedParcelsCost : 0;
        var expectedTotalCost = expectedParcelsCost + expectedSpeedyShippingCost;

        // Act
        var result = CalculateOrderCostCommandHandler.Handle(command);

        // Assert
        result.Should().Match<Result<CalculateOrderCostResponse, Error>>(r =>
            r.IsSuccess
            && r.Value.SpeedyShipping == command.SpeedyShipping
            && r.Value.SpeedyShippingCost == expectedSpeedyShippingCost
            && r.Value.TotalCost == expectedTotalCost
            && r.Value.Parcels.Count == command.Parcels.Count());

        await Verify(result.Value)
            .UseDirectory(SnapshotFilesPath)
            .UseFileName(
                nameof(Handle_CommandHasParcelsWithFixedValues_ReturnsSuccessWithFixedSnapshot) +
                $"-speedyShipping_{speedyShipping}" +
                $"-overWeight1kg_{overWeight1kg}");
    }

    private static void AssertParcelsGroup(Result<CalculateOrderCostResponse, Error> result, ParcelSize selectedParcelSize, int expectedParcelsCount)
    {
        result.Value.Parcels
            .Where(p => p.ParcelSize.Equals(selectedParcelSize.ToString()) && p.ParcelCost == GetParcelPrice(selectedParcelSize))
            .Should().HaveCount(expectedParcelsCount);
    }

    private static (IEnumerable<ParcelRequest> parcels, int parcelsCount) CreateParcelFixtures(ParcelSize parcelSize)
    {
        var parcelsCount = new Random().Next(1, 5);
        var parcels = ParcelRequestFixtureFactory.CreateMany(parcelSize, parcelsCount);

        return (parcels, parcelsCount);
    }

    private static int GetParcelPrice(ParcelSize parcelSize)
        => parcelSize switch
        {
            ParcelSize.Small => 3,
            ParcelSize.Medium => 8,
            ParcelSize.Large => 15,
            _ => 25
        };

    private static int GetParcelPrice(ParcelSize parcelSize, bool overWeight1kg)
    {
        var basePrice = parcelSize switch
        {
            ParcelSize.Small => 3,
            ParcelSize.Medium => 8,
            ParcelSize.Large => 15,
            _ => 25
        };

        return overWeight1kg ? basePrice + 2 : basePrice;
    }
}
