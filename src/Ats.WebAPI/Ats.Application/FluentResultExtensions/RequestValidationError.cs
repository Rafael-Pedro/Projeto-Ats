using FluentResults;

namespace Ats.Application.FluentResultExtensions;

public class RequestValidationError : Error
{
    public Dictionary<string, string[]> FieldReasonDictionary { get; init; } = default!;
}
