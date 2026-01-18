using System.Diagnostics;
using Mediator;
using Microsoft.Extensions.Logging;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Iniciando requisição: {Name}", requestName);

        var timer = Stopwatch.StartNew();

        var response = await next(message, cancellationToken);

        timer.Stop();

        if (timer.ElapsedMilliseconds > 500)
            _logger.LogWarning("Requisição Lenta: {Name} levou {Elapsed}ms", requestName, timer.ElapsedMilliseconds);
        else
            _logger.LogInformation("Finalizando requisição: {Name} ({Elapsed}ms)", requestName, timer.ElapsedMilliseconds);

        return response;
    }
}