namespace CourierKata.Domain.UnitTests.ValueObjects;

using CourierKata.Domain.Errors;
using CourierKata.Domain.ValueObjects;

public class PositiveDecimalTests
{
    [Fact]
    public void From_DecimalIsNotPositive_ReturnsFailureWithArgumentError()
        => PositiveDecimal.From(
            new Random().Next(int.MinValue, 1))
        .Should().Match<PositiveDecimal>(p =>
            p.IsFailure
            && p.GetError() is ArgumentError);

    [Fact]
    public void From_DecimalIsPositive_ReturnsSuccessWithValue()
    {
        decimal arrangeValue = new Random().Next(1, int.MaxValue);

        PositiveDecimal.From(arrangeValue)
        .Should().Match<PositiveDecimal>(p =>
            p.IsSuccess
            && p.GetValue() == arrangeValue);
    }
}
