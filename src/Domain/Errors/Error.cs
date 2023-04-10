namespace CourierKata.Domain.Errors;

public class Error
{
    public readonly string Message;

    public readonly string StackTrace;

    public Error(string message)
    {
        Message = string.IsNullOrWhiteSpace(message)
            ? throw new ArgumentException($"Input {nameof(message)} must not be null, empty or white spaces.", nameof(message))
            : message;
        StackTrace = Environment.StackTrace;
    }
}
