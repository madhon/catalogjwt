namespace Catalog.API.Application.Features.ListProducts;

using Catalog.API.Application.Abstractions;

public sealed class ListProductsHandler : IRequestHandler<ListProductsRequest, ListProductsResponse>
{
    private readonly ICatalogDbContext catalogDbContext;

    public ListProductsHandler(ICatalogDbContext catalogDbContext)
    {
        this.catalogDbContext = catalogDbContext;
    }

    public async ValueTask<ListProductsResponse> Handle(ListProductsRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var totalItem = await catalogDbContext.Products
            .AsNoTracking()
            .LongCountAsync(cancellationToken);

        var itemsOnPage = await catalogDbContext.GetAllProducts(request.PageSize, request.PageIndex,  cancellationToken);

        return new ListProductsResponse(totalItem, itemsOnPage);
    }
}
