namespace CourierKata.Application.OrderCost;

using CourierKata.Domain.Errors;
using CourierKata.Domain.Models;

public static class CalculateOrderCostCommandHandler
{
    public static Result<CalculateOrderCostResponse, Error> Handle(CalculateOrderCostCommand calculateOrderCostCommand)
    {
        var parcels = calculateOrderCostCommand.ToParcels();

        var validationResult = Parcel.Validate(parcels);
        if (validationResult.IsFailure)
            return validationResult.Error;

        return parcels.ToOrderCostResponse();
    }
}
