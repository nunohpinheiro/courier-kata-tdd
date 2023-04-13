namespace CourierKata.Application.UnitTests.Discounts;

using CourierKata.Application.Discounts;
using CourierKata.Domain.Models;
using CourierKata.Tests.Common.Fixtures;
using System.Collections.Generic;
using System.Linq;

public class DiscountFactoryTests
{
    [Theory]
    [InlineData(ParcelSize.Small, 4, 4, 2, 3, "Small")]
    [InlineData(ParcelSize.Medium, 3, 3, 0, 8, "Medium")]
    public void CreateDiscounts_ParcelsEligibleForSmallOrMediumDiscount_PlusMixedDiscount_ReturnsMatchingDiscounts(
        ParcelSize parcelSize,
        int discountGroups,
        int expectedSmallMediumDiscountsNumber,
        int expectedMixedDiscountsNumber,
        int expectedDiscountsValue,
        string smallMediumDiscountsDescription)
    {
        // Arrange
        List<Parcel> parcels = CreateParcelGroups(parcelSize, discountGroups);

        // Act
        var discounts = DiscountFactory.CreateDiscounts(parcels);

        // Assert
        discounts.Select(d => d.Value)
            .Should().AllBeEquivalentTo(expectedDiscountsValue);
        discounts.Where(d => d.Description.StartsWith(smallMediumDiscountsDescription))
            .Should().HaveCount(expectedSmallMediumDiscountsNumber);
        discounts.Where(d => d.Description.StartsWith("Mixed"))
            .Should().HaveCount(expectedMixedDiscountsNumber);
    }

    [Fact]
    public void CreateDiscounts_ParcelsNotEligibleForDiscount_ReturnsEmpty()
    {
        // Arrange
        var parcels = ParcelFixtureFactory.CreateMany(
            new Fixture().Create<ParcelSize>(),
            new Random().Next(1, 3));

        // Act
        var discounts = DiscountFactory.CreateDiscounts(parcels.ToList());

        // Assert
        discounts.Should().BeEmpty();
    }

    [Theory, MemberData(nameof(EmptyParcels))]
    public void CreateDiscounts_ParcelsAreEmpty_ReturnsEmpty(IEnumerable<Parcel> parcels)
    {
        // Act
        var discounts = DiscountFactory.CreateDiscounts(parcels?.ToList()!);

        // Assert
        discounts.Should().BeEmpty();
    }

    public static IEnumerable<object[]> EmptyParcels =>
        new List<object[]>
        {
            new object[]
            {
                null!
            },
            new object[]
            {
                Enumerable.Empty<Parcel>().ToList()
            },
        };

    private static List<Parcel> CreateParcelGroups(ParcelSize parcelSize, int discountGroups)
    {
        List<Parcel> parcels = new();

        for (int i = 0; i < discountGroups; i++)
        {
            var parcelGroup = ParcelFixtureFactory.CreateMany(parcelSize, discountGroups);
            parcels.AddRange(parcelGroup);
        }

        return parcels;
    }
}
