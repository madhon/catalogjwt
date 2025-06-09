namespace Catalog.API.Application.Features.ListProducts;

using Catalog.API.Application.Abstractions;
using Mediator;

public sealed record ListProductsRequest(int PageIndex, int PageSize) : IFusionCacheRequest<ListProductsResponse>
{
    public string CacheKey => $"products-all-{PageIndex}-{PageSize}";

    public IEnumerable<string>? Tags => ["products-all"];
}