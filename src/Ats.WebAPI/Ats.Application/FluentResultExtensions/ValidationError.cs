using FluentResults;

namespace Ats.Application.FluentResultExtensions;

public class ValidationError : Error
{
    public ValidationError(string message) : base(message)
    {
        Metadata.Add("ErrorCode", "VALIDATION_ERROR");
    }
}