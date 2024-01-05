namespace Catalog.API.Application.Features.ListProducts
{
    public sealed record ListProductsResponse(long TotalItems, IList<Product> Items)
    {
    }
}
