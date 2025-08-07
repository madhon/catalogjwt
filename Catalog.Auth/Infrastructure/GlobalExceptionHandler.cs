namespace Catalog.Auth.Infrastructure;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

internal sealed partial class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        this.logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails = new()
        {
            Instance = httpContext.Request.Path,
            Title = exception.Message,
            Status = httpContext.Response.StatusCode,
        };
        LogProblemDetails(problemDetails.Title, exception);

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);

        return true;
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "{Title} - ")]
    private partial void LogProblemDetails(string title, Exception ex);
}
