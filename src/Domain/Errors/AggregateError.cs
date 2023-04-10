namespace CourierKata.Domain.Errors;

using CSharpFunctionalExtensions;

public class AggregateError : Error
{
    public readonly IReadOnlyCollection<Error> InnerErrors = Enumerable.Empty<Error>().ToList().AsReadOnly();

    public AggregateError(List<Error> innerErrors)
        : base("Several errors occurred.")
    {
        if (innerErrors?.Any() is not true)
            throw new ArgumentException($"To create an {nameof(AggregateError)}, the input of {nameof(innerErrors)} must not be null or empty.");

        InnerErrors = SelectFlatErrors(innerErrors).AsReadOnly();
    }

    public static Error GetSingleErrorOrAggregate(List<Error> errors)
        => errors.Count switch
        {
            1 => errors.Single(),
            > 1 => new AggregateError(errors),
            _ => default!
        };

    private static List<Error> SelectFlatErrors(List<Error> errors)
    {
        List<Error> flatErrors = new();

        foreach (var error in errors)
        {
            if (error is AggregateError aggError)
                flatErrors.AddRange(aggError.InnerErrors);
            else flatErrors.Add(error);
        }

        return flatErrors;
    }
}
