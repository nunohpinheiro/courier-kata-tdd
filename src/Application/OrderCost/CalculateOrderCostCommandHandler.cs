namespace CourierKata.Application.OrderCost;

using CourierKata.Domain.Errors;
using CourierKata.Domain.Models;

public static class CalculateOrderCostCommandHandler
{
    public static Result<CalculateOrderCostResponse, Error> Handle(CalculateOrderCostCommand calculateOrderCostCommand)
    {
        var order = calculateOrderCostCommand.ToOrder();

        var validationResult = order.Validate();
        if (validationResult.IsFailure)
            return validationResult.Error;

        return order.ToOrderCostResponse();
    }
}
