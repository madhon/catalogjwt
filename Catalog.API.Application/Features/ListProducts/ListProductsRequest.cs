namespace Catalog.API.Application.Features.ListProducts
{
    using Mediator;

    public sealed record ListProductsRequest(int PageIndex, int PageSize) : IRequest<ListProductsResponse>
    {
    }
}
