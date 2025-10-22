namespace Catalog.API.Application.Features.ListProducts;

using System.Globalization;
using Catalog.API.Application.Abstractions;

public sealed record ListProductsRequest(int PageIndex, int PageSize) : IFusionCacheRequest<ListProductsResponse>
{
    public string CacheKey => string.Create(CultureInfo.InvariantCulture, $"products-all-{PageIndex}-{PageSize}");

    public IEnumerable<string>? Tags => ["products-all"];
}