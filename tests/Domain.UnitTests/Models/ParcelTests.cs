namespace CourierKata.Domain.UnitTests.Models;

using CourierKata.Domain.Errors;
using CourierKata.Domain.Models;
using CourierKata.Domain.Types;
using CourierKata.Tests.Common.Fixtures;

public class ParcelTests
{
    [Theory, MemberData(nameof(ParcelsWithExpectedSizes))]
    public void GetCost_ParcelHasSpecificSize_ReturnsMatchingParcelCost(Parcel parcel, ParcelSize expectedParcelSize)
        => parcel.GetCost()
        .Should().Be(GetParcelPrice(expectedParcelSize));

    [Theory, MemberData(nameof(ParcelsWithExpectedSizes))]
    public void GetSize_ParcelHasSpecificSize_ReturnsMatchingParcelSize(Parcel parcel, ParcelSize expectedParcelSize)
        => parcel.GetSize()
        .Should().Be(expectedParcelSize);

    public static IEnumerable<object[]> ParcelsWithExpectedSizes =>
        new List<object[]>
        {
            new object[] { ParcelFixtureFactory.Create(ParcelSize.Small), ParcelSize.Small },
            new object[] { ParcelFixtureFactory.Create(ParcelSize.Medium), ParcelSize.Medium },
            new object[] { ParcelFixtureFactory.Create(ParcelSize.Large), ParcelSize.Large },
            new object[] { ParcelFixtureFactory.Create(ParcelSize.ExtraLarge), ParcelSize.ExtraLarge },
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
                new Random().Next(1, int.MaxValue)) },
            new object[] { new Parcel(
                new Random().Next(1, int.MaxValue),
                new Random().Next(int.MinValue, 1),
                new Random().Next(1, int.MaxValue)) },
            new object[] { new Parcel(
                new Random().Next(1, int.MaxValue),
                new Random().Next(1, int.MaxValue),
                new Random().Next(int.MinValue, 1)) }
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
                        new Random().Next(1, int.MaxValue))
                }
            },
            new object[]
            {
                new List<Parcel>(2)
                {
                    new Parcel(
                        new Random().Next(1, int.MaxValue),
                        new Random().Next(int.MinValue, 1),
                        new Random().Next(1, int.MaxValue)),
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

    private static int GetParcelPrice(ParcelSize parcelSize)
        => parcelSize switch
        {
            ParcelSize.Small => 3,
            ParcelSize.Medium => 8,
            ParcelSize.Large => 15,
            _ => 25
        };
}
