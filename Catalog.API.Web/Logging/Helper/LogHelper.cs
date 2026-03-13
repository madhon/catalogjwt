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
        var path = ctx.Request.Path;
        return path.StartsWithSegments("/healthz", StringComparison.OrdinalIgnoreCase)
               || path.StartsWithSegments("/alive", StringComparison.OrdinalIgnoreCase);
    }
}