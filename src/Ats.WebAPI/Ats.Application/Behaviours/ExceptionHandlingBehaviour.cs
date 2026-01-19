using Ats.Application.FluentResultExtensions;
using Ats.Domain.Exceptions;
using FluentResults;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Ats.Application.Behaviours;

public class ExceptionHandlingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    private readonly ILogger<ExceptionHandlingBehaviour<TRequest, TResponse>> _logger;

    public ExceptionHandlingBehaviour(ILogger<ExceptionHandlingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(TRequest message,  MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (Exception ex)
        {
            var response = new TResponse();

            if (ex is DomainException)
            {
                _logger.LogWarning("Violação de regra de negócio: {Message}", ex.Message);

                response.Reasons.Add(new ValidationError(ex.Message));

                return response;
            }

            const string errorMessage = "An unhandled exception has occurred";
            _logger.LogError(ex, "{ErrorMessage}: {Message}", errorMessage, ex.Message);

            response.Reasons.Add(new Error(errorMessage).CausedBy(ex));
            return response;
        }
    }
}