namespace CourierKata.Domain.Errors;

public class OrderParcelsCannotBeOverwrittenError : InvalidOperationError
{
    public OrderParcelsCannotBeOverwrittenError()
        : base("Order already has parcels, so they cannot be overwritten.") { }
}
