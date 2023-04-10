namespace CourierKata.Domain.Errors;

public class ArgumentCollectionIsEmptyError : ArgumentError
{
    public ArgumentCollectionIsEmptyError(string elementName)
        : base($"Collection of {elementName} elements is empty, but it should have items.") { }
}
