namespace Catalog.API.Application.Behaviours;

using Catalog.API.Application.Abstractions;
using Microsoft.Extensions.Logging;

public sealed partial class FusionCacheBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IFusionCacheRequest<TResponse>
{
    private readonly IFusionCache _fusionCache;
    private readonly ILogger<FusionCacheBehaviour<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FusionCacheBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="fusionCache">The FusionCache instance.</param>
    /// <param name="logger">The logger instance.</param>
    public FusionCacheBehaviour(
        IFusionCache fusionCache,
        ILogger<FusionCacheBehaviour<TRequest, TResponse>> logger
    )
    {
        _fusionCache = fusionCache;
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        LogHandlingRequest(typeof(TRequest).Name, message.CacheKey);
        var response = await _fusionCache.GetOrSetAsync<TResponse>(
            message.CacheKey,
            async (ctx, token) => await next(message, token),
            tags: message.Tags,
            token: cancellationToken
        ).ConfigureAwait(false);

        return response;
    }

    [LoggerMessage(
        eventId: 223343,
        level: LogLevel.Information,
        message: "Handling request of type {requestType} with cache key {cacheKey}"
    )]
    private partial void LogHandlingRequest(string requestType, string cacheKey);
}