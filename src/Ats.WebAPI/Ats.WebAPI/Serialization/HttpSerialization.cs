using Ats.Application.FluentResultExtensions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Ats.WebAPI.Serialization;

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
            var problemDetails = new ProblemDetails
            {
                Detail = applicationError.Message,
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden"
            };

            return new ObjectResult(problemDetails) { StatusCode = StatusCodes.Status403Forbidden };
        }

        if (firstError is RequestValidationError requestValidationError)
        {
            var validationProblemDetails = new ValidationProblemDetails(requestValidationError.FieldReasonDictionary)
            {
                Detail = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error"
            };

            return new ObjectResult(validationProblemDetails) { StatusCode = StatusCodes.Status400BadRequest };
        }

        if (firstError is ValidationError domainValidationError)
        {
            var problemDetails = new ProblemDetails
            {
                Detail = domainValidationError.Message,
                Status = StatusCodes.Status400BadRequest,
                Title = "Business Rule Violation"
            };

            return new ObjectResult(problemDetails) { StatusCode = StatusCodes.Status400BadRequest };
        }

        if (firstError is NotFoundError notFoundError)
        {
            var problemDetails = new ProblemDetails
            {
                Detail = notFoundError.Message,
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found"
            };

            return new ObjectResult(problemDetails) { StatusCode = StatusCodes.Status404NotFound };
        }

        var internalErrorProblemDetails = new ProblemDetails
        {
            Detail = firstError?.Message,
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
        };

        return new ObjectResult(internalErrorProblemDetails) { StatusCode = StatusCodes.Status500InternalServerError };
    }
}