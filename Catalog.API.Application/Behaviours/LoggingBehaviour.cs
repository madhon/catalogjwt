namespace Catalog.API.Application.Behaviours;

using Mediator;
using Microsoft.Extensions.Logging;

public partial class LoggingBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly ILogger<LoggingBehaviour<TMessage, TResponse>> logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TMessage, TResponse>> logger)
    {
        this.logger = logger;
    }

    public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, TResponse> next)
    {
        var requestName = typeof(TMessage).Name;
        LogRequestName(requestName);
        return await next(message, cancellationToken).ConfigureAwait(false);
    }

    [LoggerMessage(10000, LogLevel.Information, "Request: {requestName}")]
    private partial void LogRequestName(string requestName);
}