namespace CourierKata.Domain.UnitTests.Models;

using CourierKata.Domain.Errors;
using CourierKata.Domain.Models;
using CourierKata.Domain.Types;
using CourierKata.Tests.Common.Fixtures;

public class OrderTests
{
    [Theory, MemberData(nameof(EmptyDiscounts))]
    public void SetDiscounts_DiscountsAreEmpty_ReturnsSuccess_OrderDiscountsRemainEmpty(List<Discount> discounts)
    {
        // Arrange
        Order order = new(new List<Parcel>(), new Fixture().Create<bool>());

        // Act
        var setResult = order.SetDiscounts(discounts);

        // Assert
        setResult.Should().Match<Result<Success, Error>>(r => r.IsSuccess);
        order.Discounts.Should().BeEmpty();
    }

    public static IEnumerable<object[]> EmptyDiscounts =>
        new List<object[]>
        {
            new object[]
            {
                null!
            },
            new object[]
            {
                Enumerable.Empty<Discount>().ToList()
            },
        };

    [Fact]
    public void SetDiscounts_OrderDiscountsAlreadyFilled_ReturnsFailureWithError()
    {
        // Arrange
        Order order = new(new List<Parcel>(), new Fixture().Create<bool>());
        order.SetDiscounts(DiscountFixtureFactory.CreateMany().ToList());

        // Act
        var setResult = order.SetDiscounts(DiscountFixtureFactory.CreateMany().ToList());

        // Assert
        setResult.Should().Match<Result<Success, Error>>(r =>
            r.IsFailure
            && r.Error is OrderParcelsCannotBeOverwrittenError);
    }

    [Fact]
    public void SetDiscounts_ValidInputDiscounts_ReturnsSuccess_DiscountsAreSet()
    {
        // Arrange
        Order order = new(new List<Parcel>(), new Fixture().Create<bool>());
        var discounts = DiscountFixtureFactory.CreateMany(new Random().Next(1, 100)).ToList();

        // Act
        var setResult = order.SetDiscounts(discounts);

        // Assert
        setResult.Should()
            .Match<Result<Success, Error>>(r => r.IsSuccess);
        order.Discounts.Should()
            .HaveCount(discounts.Count);
    }

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(2, 2, false)]
    [InlineData(100, 50, true)]
    public void TotalCost_OrderHasDifferentParcelsDiscountsSpeedyShipping_ReturnsMatchingCost(
        int parcelsCount, int discountsCount, bool speedyShipping)
    {
        // Arrange
        var parcels = (parcelsCount > 0
            ? ParcelFixtureFactory.CreateMany(parcelsCount: parcelsCount)
            : Enumerable.Empty<Parcel>())
            .ToList();

        var discounts = (discountsCount > 0
            ? DiscountFixtureFactory.CreateMany(discountsCount)
            : Enumerable.Empty<Discount>())
            .ToList();

        // Act
        Order order = new(parcels, speedyShipping);
        order.SetDiscounts(discounts.ToList());
        var totalCost = order.TotalCost;

        // Assert
        totalCost.Should()
            .Be(GetExpectedCost(parcels, discounts, speedyShipping));
    }

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
                            new Random().Next(1, int.MaxValue),
                            new Random().Next(1, int.MaxValue),
                            new Fixture().Create<bool>())
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
                            new Random().Next(1, int.MaxValue),
                            new Random().Next(int.MinValue, 1),
                            new Random().Next(1, int.MaxValue),
                            new Fixture().Create<bool>()),
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

    private static int GetExpectedCost(
        IEnumerable<Parcel> parcels, IEnumerable<Discount> discounts, bool speedyShipping)
    {
        var baseCost = parcels.Sum(p => p.Cost) - discounts.Sum(d => d.Value);
        return speedyShipping
            ? baseCost * 2
            : baseCost;
    }
}
