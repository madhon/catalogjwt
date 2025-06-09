namespace Catalog.API.Application.Abstractions;

public interface IFusionCacheRefreshRequest<out TResponse> : IRequest<TResponse>
{
    /// <summary>
    /// Gets the cache key associated with the request.
    /// </summary>
    string CacheKey => string.Empty;

    /// <summary>
    /// Gets the tags associated with the cache entry.
    /// </summary>
    IEnumerable<string>? Tags { get; }
}