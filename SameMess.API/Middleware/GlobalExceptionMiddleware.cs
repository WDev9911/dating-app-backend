using Microsoft.AspNetCore.Mvc;
using SameMess.Domain.Exceptions;

namespace SameMess.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await WriteProblemAsync(context, ex);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        var (status, title) = exception switch
        {
            UnauthorizedException   => (StatusCodes.Status401Unauthorized,    "Unauthorized"),
            ForbiddenException      => (StatusCodes.Status403Forbidden,       "Forbidden"),
            NotFoundException       => (StatusCodes.Status404NotFound,        "Not Found"),
            ConflictException       => (StatusCodes.Status409Conflict,        "Conflict"),
            AppException            => (StatusCodes.Status400BadRequest,      "Bad Request"),
            NotImplementedException => (StatusCodes.Status501NotImplemented,  "Not Implemented"),
            _                       => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        var problem = new ProblemDetails
        {
            Status   = status,
            Title    = title,
            Detail   = status == StatusCodes.Status500InternalServerError
                           ? "An unexpected error occurred. Please try again later."
                           : exception.Message,
            Instance = $"{context.Request.Method} {context.Request.Path}",
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;

        if (_env.IsDevelopment() && status == StatusCodes.Status500InternalServerError)
            problem.Extensions["exception"] = exception.ToString();

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode  = status;

        await context.Response.WriteAsJsonAsync(problem);
    }
}
