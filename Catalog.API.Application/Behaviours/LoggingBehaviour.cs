namespace Catalog.API.Application.Behaviours
{
    using Mediator;
    using Microsoft.Extensions.Logging;

    public class LoggingBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
        where TMessage : IMessage
    {
        private readonly ILogger<TMessage> logger;

        public LoggingBehaviour(ILogger<TMessage> logger)
        {
            this.logger = logger;
        }

        public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, TResponse> next)
        {

            var requestName = typeof(TMessage).Name;

            logger.LogInformation("Request: {Name}", requestName);

            return await next(message, cancellationToken).ConfigureAwait(false);
        }
    }
}
