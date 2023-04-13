namespace CourierKata.Application.Discounts;

using CourierKata.Domain.Models;

internal class MixedParcelManiaStrategy : ParcelDiscountByChunksStrategy
{
    protected internal override string Description => "Mixed parcel mania! Every 5th parcel in an order is free!";

    protected internal override int ParcelChunkSize => 5;

    protected internal override bool ParcelSelector(Parcel parcel) => true;
}
