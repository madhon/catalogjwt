namespace Catalog.API.Web.Logging.Helper;

using Serilog.Events;

internal static class LogHelper
{
    public static LogEventLevel ExcludeHealthChecks(HttpContext ctx, double _, Exception? ex)
    {
        ArgumentNullException.ThrowIfNull(ctx);

        if (ex != null)
        {
            return LogEventLevel.Error;
        }

        if (ctx.Response.StatusCode > 499)
        {
            return LogEventLevel.Error;
        }

        return IsHealthCheckEndpoint(ctx) // Not an error, check if it was a health check
            ? LogEventLevel.Verbose // Was a health check, use Verbose
            : LogEventLevel.Information;
    }

    private static bool IsHealthCheckEndpoint(HttpContext ctx)
    {
        var endpoint = ctx.GetEndpoint();
        if (endpoint is not null)
        {
            return string.Equals(
                endpoint.DisplayName,
                "Health checks",
                StringComparison.Ordinal);
        }
        // No endpoint, so not a health check endpoint
        return false;
    }
}