namespace CourierKata.Domain.UnitTests.Models;

using CourierKata.Domain.Errors;
using CourierKata.Domain.Models;
using CourierKata.Domain.Types;
using CourierKata.Tests.Common.Fixtures;

public class ParcelTests
{
    [Theory, MemberData(nameof(ParcelsWithExpectedSizesAndWeights))]
    public void Cost_ParcelHasSpecificSizeAndWeight_ReturnsMatchingParcelCost_DependsOnOverWeight(
        Parcel parcel, ParcelSize expectedParcelSize, int weight, bool isOverWeight, bool heavyParcel)
        => parcel.Should().Match<Parcel>(p =>
            p.Cost == GetParcelPrice(expectedParcelSize, weight, heavyParcel)
            && p.IsOverWeight == isOverWeight
            && p.HeavyParcel == heavyParcel);

    public static IEnumerable<object[]> ParcelsWithExpectedSizesAndWeights =>
        new List<object[]>
        {
            new object[] { ParcelFixtureFactory.Create(ParcelSize.Small, 1, false), ParcelSize.Small, 1, false, false },
            new object[] { ParcelFixtureFactory.Create(ParcelSize.Small, 78, true), ParcelSize.Small, 78, true, true },
            new object[] { ParcelFixtureFactory.Create(ParcelSize.Medium, 50, true), ParcelSize.Medium, 50, false, true },
            new object[] { ParcelFixtureFactory.Create(ParcelSize.Medium, 4, false), ParcelSize.Medium, 4, true, false },
            new object[] { ParcelFixtureFactory.Create(ParcelSize.Large, 6, false), ParcelSize.Large, 6, false, false },
            new object[] { ParcelFixtureFactory.Create(ParcelSize.Large, 51, true), ParcelSize.Large, 51, true, true },
            new object[] { ParcelFixtureFactory.Create(ParcelSize.ExtraLarge, 10, false), ParcelSize.ExtraLarge, 10, false, false },
            new object[] { ParcelFixtureFactory.Create(ParcelSize.ExtraLarge, 101, true), ParcelSize.ExtraLarge, 101, true, true }
        };

    [Theory, MemberData(nameof(ParcelsWithExpectedSizes))]
    public void Size_ParcelHasSpecificSize_ReturnsMatchingParcelSize(
        Parcel parcel, ParcelSize expectedParcelSize)
        => parcel.Size
        .Should().Be(expectedParcelSize);

    public static IEnumerable<object[]> ParcelsWithExpectedSizes =>
        new List<object[]>
        {
            new object[] { ParcelFixtureFactory.Create(ParcelSize.Small), ParcelSize.Small },
            new object[] { ParcelFixtureFactory.Create(ParcelSize.Medium), ParcelSize.Medium },
            new object[] { ParcelFixtureFactory.Create(ParcelSize.Large), ParcelSize.Large },
            new object[] { ParcelFixtureFactory.Create(ParcelSize.ExtraLarge), ParcelSize.ExtraLarge }
        };

    [Theory, MemberData(nameof(ParcelsWithOneInvalidProperty))]
    public void Validate_ParcelHasOneInvalidProperty_ReturnsArgumentError(Parcel parcel)
        => parcel.Validate()
        .Should().Match<Result<Success, Error>>(r =>
            r.IsFailure
            && r.Error is ArgumentError);

    public static IEnumerable<object[]> ParcelsWithOneInvalidProperty =>
        new List<object[]>
        {
            new object[] { new Parcel(
                new Random().Next(int.MinValue, 1),
                new Random().Next(1, int.MaxValue),
                new Random().Next(1, int.MaxValue),
                new Random().Next(1, int.MaxValue),
                new Fixture().Create<bool>()) },
            new object[] { new Parcel(
                new Random().Next(1, int.MaxValue),
                new Random().Next(int.MinValue, 1),
                new Random().Next(1, int.MaxValue),
                new Random().Next(1, int.MaxValue),
                new Fixture().Create<bool>()) },
            new object[] { new Parcel(
                new Random().Next(1, int.MaxValue),
                new Random().Next(1, int.MaxValue),
                new Random().Next(int.MinValue, 1),
                new Random().Next(1, int.MaxValue),
                new Fixture().Create<bool>()) },
            new object[] { new Parcel(
                new Random().Next(1, int.MaxValue),
                new Random().Next(1, int.MaxValue),
                new Random().Next(1, int.MaxValue),
                new Random().Next(int.MinValue, 1),
                new Fixture().Create<bool>()) }
        };

    [Fact]
    public void Validate_ParcelHasSeveralInvalidProperties_ReturnsAggregateError()
        => ParcelFixtureFactory.CreateInvalid()
        .Validate()
        .Should().Match<Result<Success, Error>>(r =>
            r.IsFailure
            && r.Error is AggregateError);

    [Theory]
    [InlineData(ParcelSize.Small)]
    [InlineData(ParcelSize.Medium)]
    [InlineData(ParcelSize.Large)]
    [InlineData(ParcelSize.ExtraLarge)]
    public void Validate_ParcelHasValidProperties_ReturnsSuccess(ParcelSize parcelSize)
        => ParcelFixtureFactory.Create(parcelSize)
        .Validate()
        .Should().Match<Result<Success, Error>>(r => r.IsSuccess);

    [Theory, MemberData(nameof(ParcelsListWithOneInvalidProperty))]
    public void Validate_ParcelsList_OneParcelHasOneInvalidProperty_ReturnsArgumentError(List<Parcel> parcels)
        => Parcel.Validate(parcels)
        .Should().Match<Result<Success, Error>>(r =>
            r.IsFailure
            && r.Error is ArgumentError);

    public static IEnumerable<object[]> ParcelsListWithOneInvalidProperty =>
        new List<object[]>
        {
            new object[]
            {
                new List<Parcel>(1)
                {
                    new Parcel(
                        new Random().Next(int.MinValue, 1),
                        new Random().Next(1, int.MaxValue),
                        new Random().Next(1, int.MaxValue),
                        new Random().Next(1, int.MaxValue),
                        new Fixture().Create<bool>())
                }
            },
            new object[]
            {
                new List<Parcel>(2)
                {
                    new Parcel(
                        new Random().Next(1, int.MaxValue),
                        new Random().Next(1, int.MaxValue),
                        new Random().Next(int.MinValue, 1),
                        new Random().Next(1, int.MaxValue),
                        new Fixture().Create<bool>()),
                    ParcelFixtureFactory.Create()
                }
            },
        };

    [Theory, MemberData(nameof(ParcelsListWithSeveralInvalidProperties))]
    public void Validate_ParcelsList_ParcelsHaveSeveralInvalidProperties_ReturnsAggregateError(List<Parcel> parcels)
        => Parcel.Validate(parcels)
        .Should().Match<Result<Success, Error>>(r =>
            r.IsFailure
            && r.Error is AggregateError);

    public static IEnumerable<object[]> ParcelsListWithSeveralInvalidProperties =>
        new List<object[]>
        {
            new object[]
            {
                new List<Parcel>(1)
                {
                    ParcelFixtureFactory.CreateInvalid()
                }
            },
            new object[]
            {
                new List<Parcel>(2)
                {
                    ParcelFixtureFactory.CreateInvalid(),
                    ParcelFixtureFactory.CreateInvalid()
                }
            },
        };

    [Theory]
    [InlineData(ParcelSize.Small)]
    [InlineData(ParcelSize.Medium)]
    [InlineData(ParcelSize.Large)]
    [InlineData(ParcelSize.ExtraLarge)]
    public void Validate_ParcelsList_ParcelsHaveValidProperties_ReturnsSuccess(ParcelSize parcelSize)
        => Parcel.Validate(
            ParcelFixtureFactory.CreateMany(parcelSize).ToList())
        .Should().Match<Result<Success, Error>>(r => r.IsSuccess);

    private static int GetParcelPrice(ParcelSize parcelSize, int weight, bool heavyParcel)
        => parcelSize switch
        {
            ParcelSize.Small => 3 + GetWeightCost(parcelSize, weight, heavyParcel),
            ParcelSize.Medium => 8 + GetWeightCost(parcelSize, weight, heavyParcel),
            ParcelSize.Large => 15 + GetWeightCost(parcelSize, weight, heavyParcel),
            _ => 25 + GetWeightCost(parcelSize, weight, heavyParcel)
        };

    private static int GetHeavyParcelWeightCost(int weight)
        => 50 + (weight > 50 ? weight - 50 : 0);

    private static int GetWeightCost(ParcelSize parcelSize, int weight, bool heavyParcel)
        => heavyParcel
        ? GetHeavyParcelWeightCost(weight)
        : parcelSize switch
        {
            ParcelSize.Small when weight > 1 => GetWeightCostBySize(weight, 1),
            ParcelSize.Medium when weight > 3 => GetWeightCostBySize(weight, 3),
            ParcelSize.Large when weight > 6 => GetWeightCostBySize(weight, 6),
            ParcelSize.ExtraLarge when weight > 10 => GetWeightCostBySize(weight, 10),
            _ => 0
        };

    private static int GetWeightCostBySize(int weight, int weightBoundary)
        => 2 * (weight - weightBoundary);
}
