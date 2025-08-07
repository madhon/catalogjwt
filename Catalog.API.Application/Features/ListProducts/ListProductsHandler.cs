namespace Catalog.API.Application.Features.ListProducts;

using Catalog.API.Application.Abstractions;
using Mediator;

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
            .LongCountAsync(cancellationToken).ConfigureAwait(false);

        var itemsOnPage = catalogDbContext.GetAllProducts(request.PageSize, request.PageIndex).ToList();

#pragma warning disable S125
        //var itemsOnPage = await catalogDbContext.Products
        //    .AsNoTracking()
        //    .Where(x =>
        //        catalogDbContext.Products
        //            .OrderBy(c => c.Name)
        //            .Select(y => y.Id)
        //            .Skip(request.PageSize * request.PageIndex)
        //            .Take(request.PageSize)
        //            .Contains(x.Id))
        //    .ToListAsync(cancellationToken)
        //    .ConfigureAwait(false);
#pragma warning restore S125

        return new ListProductsResponse(totalItem, itemsOnPage);
    }
}