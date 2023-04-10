namespace CourierKata.Domain.Errors;

public class ArgumentError : Error
{
    public ArgumentError(string message) : base(message) { }
}
