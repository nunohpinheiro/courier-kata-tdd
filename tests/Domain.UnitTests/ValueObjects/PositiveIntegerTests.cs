namespace CourierKata.Domain.UnitTests.ValueObjects;

using CourierKata.Domain.Errors;
using CourierKata.Domain.ValueObjects;

public class PositiveIntegerTests
{
    [Fact]
    public void From_IntIsNotPositive_ReturnsFailureWithArgumentError()
        => PositiveInteger.From(
            new Random().Next(int.MinValue, 1))
        .Should().Match<PositiveInteger>(p =>
            p.IsFailure
            && p.GetError() is ArgumentError);

    [Fact]
    public void From_IntIsPositive_ReturnsSuccessWithValue()
    {
        int arrangeValue = new Random().Next(1, int.MaxValue);

        PositiveInteger.From(arrangeValue)
        .Should().Match<PositiveInteger>(p =>
            p.IsSuccess
            && p.GetValue() == arrangeValue);
    }
}
