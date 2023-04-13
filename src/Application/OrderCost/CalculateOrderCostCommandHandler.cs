namespace CourierKata.Application.OrderCost;

using CourierKata.Application.Discounts;
using CourierKata.Domain.Errors;

public static class CalculateOrderCostCommandHandler
{
    public static Result<CalculateOrderCostResponse, Error> Handle(CalculateOrderCostCommand calculateOrderCostCommand)
    {
        var order = calculateOrderCostCommand.ToOrder();

        var validationResult = order.Validate();
        if (validationResult.IsFailure)
            return validationResult.Error;

        var setDiscountsResult = order.SetDiscounts(
            DiscountFactory.CreateDiscounts(order.Parcels.ToList()));
        if (setDiscountsResult.IsFailure)
            return setDiscountsResult.Error;

        return order.ToOrderCostResponse();
    }
}
