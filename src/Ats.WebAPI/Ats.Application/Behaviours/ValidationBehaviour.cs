using FluentResults;
using FluentValidation;
using Mediator;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next(message, cancellationToken);

        var context = new ValidationContext<TRequest>(message);

        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Count != 0)
        {
            var result = new TResponse();
            foreach (var failure in failures)
            {
                result.Reasons.Add(new Error(failure.ErrorMessage)
                    .WithMetadata("Property", failure.PropertyName));
            }
            return result;
        }

        return await next(message, cancellationToken);
    }
}