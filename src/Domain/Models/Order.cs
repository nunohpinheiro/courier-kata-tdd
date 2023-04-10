namespace CourierKata.Domain.Models;

using CourierKata.Domain.Errors;
using CourierKata.Domain.Types;
using CSharpFunctionalExtensions;

public record Order
{
    public IReadOnlyCollection<Parcel> Parcels { get; init; } = Enumerable.Empty<Parcel>().ToList().AsReadOnly();
    public bool SpeedyShipping { get; init; }

    public Order(IEnumerable<Parcel>? parcels, bool? speedyShipping = null)
    {
        Parcels = (parcels ?? Enumerable.Empty<Parcel>()).ToList().AsReadOnly();
        SpeedyShipping = speedyShipping ?? false;
    }

    public Result<Success, Error> Validate()
        => Parcel.Validate(Parcels.ToList());
}
