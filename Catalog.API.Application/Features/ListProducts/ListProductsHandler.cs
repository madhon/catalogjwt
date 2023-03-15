namespace Catalog.API.Application.Features.ListProducts
{
    using Catalog.API.Application.Common;
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
            var totalItem = await catalogDbContext.Products
                .AsNoTracking()
                .LongCountAsync(cancellationToken).ConfigureAwait(false);

            var itemsOnPage = await catalogDbContext.Products
                .AsNoTracking()
                .Where(x =>
                    catalogDbContext.Products
                        .OrderBy(c => c.Name)
                        .Select(y => y.Id)
                        .Skip(request.PageSize * request.PageIndex)
                        .Take(request.PageSize)
                        .Contains(x.Id))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new ListProductsResponse(totalItem, itemsOnPage);
        }
    }
}
