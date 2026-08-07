namespace Catalog.API.Application.Behaviours;

using System.Diagnostics;
using Catalog.API.Application.Diagnostics;
using Mediator;
using Microsoft.Extensions.Logging;

public sealed partial class PerformanceBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private const long SlowRequestThresholdMs = 500;

    private static readonly string HandlerName = typeof(TMessage).Name;
    private readonly MediatorMetrics metrics;

    private readonly ILogger<PerformanceBehaviour<TMessage, TResponse>> logger;

    public PerformanceBehaviour(MediatorMetrics metrics, ILogger<PerformanceBehaviour<TMessage, TResponse>> logger)
    {
        this.metrics = metrics;
        this.logger = logger;
    }

    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        using var activity = MediatorActivity.Source.StartActivity(
            HandlerName,
            ActivityKind.Internal);

        var start = Stopwatch.GetTimestamp();

        try
        {
            var response = await next(message, cancellationToken).ConfigureAwait(false);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return response;
        }
        catch (Exception e)
        {
            activity?.SetStatus(ActivityStatusCode.Error, e.Message);
            activity?.AddException(e);
            throw;
        }
        finally
        {
            var elapsedMs = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
            metrics.RecordHandlerDuration(HandlerName, elapsedMs);
            activity?.SetTag("mediator.handler", HandlerName);
            activity?.SetTag("mediator.duration_ms", elapsedMs);

            if (elapsedMs > SlowRequestThresholdMs)
            {
                LogLongRunningRequest(typeof(TMessage).Name, elapsedMs);
            }
        }
    }

    [LoggerMessage(10001, LogLevel.Warning,
        "{RequestName} long running request ({ElapsedMilliseconds} ms)")]
    private partial void LogLongRunningRequest(string requestName, double elapsedMilliseconds);
}
