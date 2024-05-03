namespace Catalog.Auth.Infrastructure;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public sealed partial class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        this.logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();
        problemDetails.Instance = httpContext.Request.Path;
        problemDetails.Title = exception.Message;
        problemDetails.Status = httpContext.Response.StatusCode;
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
