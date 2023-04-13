namespace CourierKata.Domain.Errors;

public class InvalidOperationError : Error
{
    public InvalidOperationError(string message) : base(message) { }
}
