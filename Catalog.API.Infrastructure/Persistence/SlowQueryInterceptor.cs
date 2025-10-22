namespace Catalog.API.Infrastructure.Persistence;

using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

public sealed partial class SlowQueryInterceptor : DbCommandInterceptor
{
    private const int SlowQueryThreshold = 200;
    private readonly ILogger<SlowQueryInterceptor> logger;

    public SlowQueryInterceptor(ILogger<SlowQueryInterceptor> logger)
    {
        this.logger = logger;
    }

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(eventData);

        if (eventData.Duration.TotalMilliseconds > SlowQueryThreshold)
        {
            LogSlowQuery(eventData.Duration.TotalMilliseconds, command.CommandText);
        }

        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(eventData);

        if (eventData.Duration.TotalMilliseconds > SlowQueryThreshold)
        {
            LogSlowQuery(eventData.Duration.TotalMilliseconds, command.CommandText);
        }

        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    [LoggerMessage(
        EventId = 1956, 
        Level = LogLevel.Warning,
        Message = "Slow query detected (Duration: {Duration} ms): {CommandText}")]
    private partial void LogSlowQuery(double duration, string commandText);
}