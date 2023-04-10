namespace CourierKata.Domain.Errors;

using CourierKata.Domain.Models;

public class ParcelsCollectionIsEmptyError : ArgumentCollectionIsEmptyError
{
    public ParcelsCollectionIsEmptyError()
        : base(nameof(Parcel)) { }
}
