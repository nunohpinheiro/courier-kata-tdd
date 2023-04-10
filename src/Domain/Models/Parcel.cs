namespace CourierKata.Domain.Models;

using CourierKata.Domain.Errors;
using CourierKata.Domain.Types;
using CourierKata.Domain.ValueObjects;
using CSharpFunctionalExtensions;

public record Parcel
{
    private const decimal SmallDimension = 10;
    private const decimal MediumDimension = 50;
    private const decimal LargeDimension = 100;

    public PositiveDecimal Length { get; init; }
    public PositiveDecimal Width { get; init; }
    public PositiveDecimal Height { get; init; }

    public Parcel(decimal length, decimal width, decimal height)
    {
        Length = length;
        Width = width;
        Height = height;
    }

    public int GetCost()
        => GetSize() switch
        {
            ParcelSize.Small => 3,
            ParcelSize.Medium => 8,
            ParcelSize.Large => 15,
            _ => 25
        };

    public ParcelSize GetSize()
    {
        if (IsSmall()) return ParcelSize.Small;
        if (IsMedium()) return ParcelSize.Medium;
        if (IsLarge()) return ParcelSize.Large;
        return ParcelSize.ExtraLarge;
    }

    public Result<Success, Error> Validate()
    {
        var errors =
            new List<PositiveDecimal>(3) { Length, Width, Height }
            .Where(pd => pd.IsFailure)
            .Select(pd => pd.GetError())
            .ToList();

        return GetSuccessOrErrors(errors);
    }

    public static Result<Success, Error> Validate(List<Parcel> parcels)
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

    private bool IsSmall()
        => IsSmall(Length) && IsSmall(Width) && IsSmall(Height);

    private bool IsMedium()
        => IsMedium(Length) && IsMedium(Width) && IsMedium(Height);

    private bool IsLarge()
        => IsLarge(Length) && IsLarge(Width) && IsLarge(Height);

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
}
