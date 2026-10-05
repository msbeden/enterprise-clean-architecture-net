using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Enterprise.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior to log request details and execution performance.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;
    private readonly Stopwatch _timer;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Enterprise Request Handling: {Name} {@Request}", requestName, request);

        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            _logger.LogWarning("Enterprise Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                requestName, elapsedMilliseconds, request);
        }
        else
        {
            _logger.LogInformation("Enterprise Request Completed: {Name} in {ElapsedMilliseconds} ms", requestName, elapsedMilliseconds);
        }

        return response;
    }
}
