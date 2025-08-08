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

    [LoggerMessage(EventId = 1956, Level = LogLevel.Warning, Message = "Slow query ({ms} ms:) {query}")]
    private partial void LogSlowQuery(double ms, string query);
}