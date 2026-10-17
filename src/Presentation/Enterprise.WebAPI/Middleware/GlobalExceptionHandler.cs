using Enterprise.Application.Common.Exceptions;
using Enterprise.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Enterprise.WebAPI.Middleware;

/// <summary>
/// Global exception handler implementing RFC 7807 (ProblemDetails) format.
/// Handles domain, application, and unhandled exceptions gracefully.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, problemDetails) = exception switch
        {
            ValidationException valEx => (
                StatusCodes.Status400BadRequest,
                CreateValidationProblemDetails(httpContext, valEx)
            ),
            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                CreateProblemDetails(httpContext, StatusCodes.Status404NotFound, "Resource Not Found", notFoundEx.Message)
            ),
            DomainException domainEx => (
                StatusCodes.Status400BadRequest,
                CreateProblemDetails(httpContext, StatusCodes.Status400BadRequest, "Domain Business Error", domainEx.Message)
            ),
            UnauthorizedAccessException unauthEx => (
                StatusCodes.Status401Unauthorized,
                CreateProblemDetails(httpContext, StatusCodes.Status401Unauthorized, "Unauthorized", unauthEx.Message)
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                CreateProblemDetails(httpContext, StatusCodes.Status500InternalServerError, "Server Error", exception.Message)
            )
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails CreateProblemDetails(HttpContext context, int status, string title, string detail)
    {
        return new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(HttpContext context, ValidationException exception)
    {
        var problemDetails = new ValidationProblemDetails(exception.Errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Failed",
            Detail = "One or more validation errors occurred.",
            Instance = context.Request.Path
        };

        return problemDetails;
    }
}
