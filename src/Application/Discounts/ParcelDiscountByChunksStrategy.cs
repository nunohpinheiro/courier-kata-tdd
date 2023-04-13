namespace CourierKata.Application.Discounts;

using CourierKata.Domain.Models;
using System.Collections.Generic;
using System.Linq;

internal abstract class ParcelDiscountByChunksStrategy
{
    protected internal abstract string Description { get; }

    protected internal abstract int ParcelChunkSize { get; }

    protected internal abstract bool ParcelSelector(Parcel parcel);

    internal IReadOnlyList<Discount> GetDiscounts(IList<Parcel> parcels)
        => parcels?
        .Where(ParcelSelector)
        .Chunk(ParcelChunkSize)
        .Where(parcelChunk => parcelChunk.Length == ParcelChunkSize)
        .Select(parcelChunk =>
            parcelChunk.MinBy(parcel => parcel.Cost)) // Within each discount, the cheapest parcel is the free one
        .Select(parcel => new Discount(parcel!.Id, parcel.Cost, Description))
        .ToList()
        ?? Enumerable.Empty<Discount>().ToList();
}
