namespace Catalog.API.Application.Behaviours;

using System.Diagnostics;
using Mediator;
using Microsoft.Extensions.Logging;

public partial class PerformanceBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly ILogger<PerformanceBehaviour<TMessage, TResponse>> logger;

    public PerformanceBehaviour(ILogger<PerformanceBehaviour<TMessage, TResponse>> logger)
    {
        this.logger = logger;
    }

    public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, TResponse> next)
    {
        Stopwatch? timer = null;

        Interlocked.Increment(ref RequestCounter.ExecutionCount);
        if (RequestCounter.ExecutionCount > 3) 
        {
            timer = Stopwatch.StartNew();
        }

        var response = await next(message, cancellationToken).ConfigureAwait(false);

        timer?.Stop();
        var elapsedMilliseconds = timer?.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TMessage).Name;
            LogLongRunningRequest(requestName, elapsedMilliseconds, message);
        }

        return response;
    }

    [LoggerMessage(10001, LogLevel.Warning, "{requestName} long running request ({ElapsedMilliseconds} milliseconds) with {message}")]
    public partial void LogLongRunningRequest(string requestName, long? elapsedMilliseconds, TMessage message);

    internal static class RequestCounter
    {
        public static int ExecutionCount;
    }
}