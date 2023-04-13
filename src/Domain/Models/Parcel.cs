namespace CourierKata.Domain.Models;

using CourierKata.Domain.Errors;
using CourierKata.Domain.Types;
using CourierKata.Domain.ValueObjects;
using CSharpFunctionalExtensions;
using System.Diagnostics;

public record Parcel
{
    private const decimal SmallDimension = 10;
    private const decimal MediumDimension = 50;
    private const decimal LargeDimension = 100;

    public Guid Id { get; } = Guid.NewGuid();
    public PositiveDecimal Length { get; init; }
    public PositiveDecimal Width { get; init; }
    public PositiveDecimal Height { get; init; }
    public PositiveInteger Weight { get; init; }
    public bool HeavyParcel { get; init; }

    public int Cost => Size switch
    {
        ParcelSize.Small => 3 + GetWeightCost(),
        ParcelSize.Medium => 8 + GetWeightCost(),
        ParcelSize.Large => 15 + GetWeightCost(),
        ParcelSize.ExtraLarge => 25 + GetWeightCost(),
        _ => throw new UnreachableException("Unexpected parcel size was provided")
    };

    public bool IsOverWeight => HeavyParcel switch
    {
        false when GetWeightCost() > 0 => true,
        true when GetWeightCost() > 50 => true,
        _ => false
    };

    public ParcelSize Size
    {
        get
        {
            if (IsSmall()) return ParcelSize.Small;
            if (IsMedium()) return ParcelSize.Medium;
            if (IsLarge()) return ParcelSize.Large;
            if (IsExtraLarge()) return ParcelSize.ExtraLarge;
            throw new UnreachableException("No size/dimension conditions were met");
        }
    }

    public Parcel(
        decimal length, decimal width, decimal height, int weight, bool heavyParcel)
    {
        Length = length;
        Width = width;
        Height = height;
        Weight = weight;
        HeavyParcel = heavyParcel;
    }

    public Result<Success, Error> Validate()
    {
        var errors =
            new List<PositiveDecimal>(3) { Length, Width, Height }
            .Where(pd => pd.IsFailure)
            .Select(pd => pd.GetError())
            .ToList();

        if (Weight.IsFailure)
            errors.Add(Weight.GetError());

        return GetSuccessOrErrors(errors);
    }

    public static Result<Success, Error> Validate(IReadOnlyList<Parcel> parcels)
    {
        if (parcels?.Any() is not true)
            return new ParcelsCollectionIsEmptyError();

        var validationErrors = parcels
            .Select(p => p.Validate())
            .Where(r => r.IsFailure)
            .Select(r => r.Error)
            .ToList();

        return GetSuccessOrErrors(validationErrors);
    }

    private int GetWeightCost()
        => HeavyParcel
        ? GetHeavyParcelWeightCost()
        : Size switch
        {
            ParcelSize.Small when Weight > 1 => GetWeightCostBySize(1),
            ParcelSize.Medium when Weight > 3 => GetWeightCostBySize(3),
            ParcelSize.Large when Weight > 6 => GetWeightCostBySize(6),
            ParcelSize.ExtraLarge when Weight > 10 => GetWeightCostBySize(10),
            _ => 0
        };

    private int GetHeavyParcelWeightCost()
        => 50 + (Weight > 50 ? Weight - 50 : 0);

    private int GetWeightCostBySize(int weightBoundary)
        => 2 * (Weight - weightBoundary);

    private bool IsSmall()
        => IsSmall(Length) && IsSmall(Width) && IsSmall(Height);

    private bool IsMedium()
        => IsMedium(Length) && IsMedium(Width) && IsMedium(Height);

    private bool IsLarge()
        => IsLarge(Length) && IsLarge(Width) && IsLarge(Height);

    private bool IsExtraLarge()
        => IsExtraLarge(Length) || IsExtraLarge(Width) || IsExtraLarge(Height);

    private static Result<Success, Error> GetSuccessOrErrors(List<Error> validationErrors)
        => validationErrors.Any() switch
        {
            true => AggregateError.GetSingleErrorOrAggregate(validationErrors.ToList()),
            _ => new Success()
        };

    private static bool IsSmall(decimal value)
        => value < SmallDimension;

    private static bool IsMedium(decimal value)
        => value is (>= SmallDimension and < MediumDimension);

    private static bool IsLarge(decimal value)
        => value is (>= MediumDimension and < LargeDimension);

    private static bool IsExtraLarge(decimal value)
        => value >= LargeDimension;
}
