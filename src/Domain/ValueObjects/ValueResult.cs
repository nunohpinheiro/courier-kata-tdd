namespace CourierKata.Domain.ValueObjects;

using CourierKata.Domain.Errors;
using CSharpFunctionalExtensions;
using ValueOf;

public class ValueResult<TValue, TThis> : ValueOf<Result<TValue, Error>, TThis>
    where TThis : ValueOf<Result<TValue, Error>, TThis>, new()
{
    public static TThis From(TValue value)
        => From(Result.Success<TValue, Error>(value));

    public bool IsFailure => Value.IsFailure;

    public bool IsSuccess => Value.IsSuccess;

    public Error GetError() => Value.Error;

    public TValue GetValue() => Value.Value;
}
