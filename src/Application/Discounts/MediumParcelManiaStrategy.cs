namespace CourierKata.Application.Discounts;

using CourierKata.Domain.Models;

internal class MediumParcelManiaStrategy : ParcelDiscountByChunksStrategy
{
    protected internal override string Description => "Medium parcel mania! Every 3rd medium parcel in an order is free!";

    protected internal override int ParcelChunkSize => 3;

    protected internal override bool ParcelSelector(Parcel parcel) => parcel.Size is ParcelSize.Medium;
}
