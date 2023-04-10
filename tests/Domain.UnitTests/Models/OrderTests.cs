namespace CourierKata.Domain.UnitTests.Models;

using CourierKata.Domain.Errors;
using CourierKata.Domain.Models;
using CourierKata.Domain.Types;
using CourierKata.Tests.Common.Fixtures;

public class OrderTests
{
    [Theory, MemberData(nameof(OrderParcelsWithOneInvalidProperty))]
    public void Validate_OneParcelHasOneInvalidProperty_ReturnsArgumentError(Order order)
        => order.Validate()
        .Should().Match<Result<Success, Error>>(r =>
            r.IsFailure
            && r.Error is ArgumentError);

    public static IEnumerable<object[]> OrderParcelsWithOneInvalidProperty =>
        new List<object[]>
        {
            new object[]
            {
                new Order(
                    new List<Parcel>(1)
                    {
                        new Parcel(
                            new Random().Next(int.MinValue, 1),
                            new Random().Next(1, int.MaxValue),
                            new Random().Next(1, int.MaxValue))
                    },
                    new Fixture().Create<bool>())
            },
            new object[]
            {
                new Order(
                    new List<Parcel>(2)
                    {
                        new Parcel(
                            new Random().Next(1, int.MaxValue),
                            new Random().Next(int.MinValue, 1),
                            new Random().Next(1, int.MaxValue)),
                        ParcelFixtureFactory.Create()
                    },
                    new Fixture().Create<bool>())
            },
        };

    [Theory, MemberData(nameof(OrderParcelsWithSeveralInvalidProperties))]
    public void Validate_ParcelsHaveSeveralInvalidProperties_ReturnsAggregateError(Order order)
        => order.Validate()
        .Should().Match<Result<Success, Error>>(r =>
            r.IsFailure
            && r.Error is AggregateError);

    public static IEnumerable<object[]> OrderParcelsWithSeveralInvalidProperties =>
        new List<object[]>
        {
            new object[]
            {
                new Order(
                    new List<Parcel>(1)
                    {
                        ParcelFixtureFactory.CreateInvalid()
                    },
                    new Fixture().Create<bool>())
            },
            new object[]
            {
                new Order(
                    new List<Parcel>(2)
                    {
                        ParcelFixtureFactory.CreateInvalid(),
                        ParcelFixtureFactory.CreateInvalid()
                    },
                    new Fixture().Create<bool>())
            },
        };

    [Theory]
    [InlineData(ParcelSize.Small)]
    [InlineData(ParcelSize.Medium)]
    [InlineData(ParcelSize.Large)]
    [InlineData(ParcelSize.ExtraLarge)]
    public void Validate_ParcelsHaveValidProperties_ReturnsSuccess(ParcelSize parcelSize)
        => new Order(
            ParcelFixtureFactory.CreateMany(parcelSize).ToList(),
            new Fixture().Create<bool>())
        .Validate()
        .Should().Match<Result<Success, Error>>(r => r.IsSuccess);
}
