namespace CourierKata.Application.Discounts;

using CourierKata.Domain.Models;

internal class SmallParcelManiaStrategy : ParcelDiscountByChunksStrategy
{
    protected internal override string Description => "Small parcel mania! Every 4th small parcel in an order is free!";

    protected internal override int ParcelChunkSize => 4;

    protected internal override bool ParcelSelector(Parcel parcel) => parcel.Size is ParcelSize.Small;
}
