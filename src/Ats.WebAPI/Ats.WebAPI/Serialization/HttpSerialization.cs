using Ats.Application.FluentResultExtensions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Serialization;

public class HttpSerialization
{
    public static IActionResult Serialize<TValue>(Result<TValue> result)
    {
        if (result is { IsSuccess: true })
            return new ObjectResult(result.Value);

        return Serialize(result.ToResult());
    }

    public static IActionResult Serialize(Result result)
    {
        if (result is { IsSuccess: true })
            return new NoContentResult();

        var firstError = result.Errors.FirstOrDefault();

        if (firstError is ApplicationError applicationError)
        {
            var validationProblemDetails = new ProblemDetails
            {
                Detail = applicationError.Message,
                Status = StatusCodes.Status403Forbidden,
                Title = applicationError.Message
            };

            return new ObjectResult(validationProblemDetails) { StatusCode = StatusCodes.Status403Forbidden };
        }

        if (firstError is RequestValidationError validationError)
        {
            var validationProblemDetails = new ValidationProblemDetails(validationError.FieldReasonDictionary)
            {
                Detail = firstError.Message,
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation error"
            };

            return new ObjectResult(validationProblemDetails) { StatusCode = StatusCodes.Status400BadRequest };
        }

        if (firstError is NotFoundError notFoundError)
        {
            var validationProblemDetails = new ProblemDetails
            {
                Detail = notFoundError.Message,
                Status = StatusCodes.Status404NotFound,
                Title = notFoundError.Message
            };

            return new ObjectResult(validationProblemDetails) { StatusCode = StatusCodes.Status404NotFound };
        }

        var problemDetails = new ProblemDetails
        {
            Detail = firstError?.Message,
            Status = StatusCodes.Status500InternalServerError,
            Title = "Unhandled error",
        };

        return new ObjectResult(problemDetails) { StatusCode = StatusCodes.Status500InternalServerError };
    }
}
