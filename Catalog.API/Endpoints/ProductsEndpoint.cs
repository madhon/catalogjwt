namespace Catalog.API.Endpoints
{
    public sealed class ProductsEndpoint : Endpoint<ProductsRequest>
    {
        private readonly CatalogContext catalogContext;

        public ProductsEndpoint(CatalogContext context)
        {
            this.catalogContext = context;
        }

        public override void Configure()
        {
            Get("products/{pageSize}/{PageIndex}");
            Summary(s =>
            {
                s.Summary = "Retrieves paged products";
                s.ExampleRequest = new ProductsRequest
                {
                    PageIndex = 0,
                    PageSize = 10
                };
            });
        }

        public override async Task HandleAsync(ProductsRequest req, CancellationToken ct)
        {
            var totalItem = await catalogContext.Product.LongCountAsync(ct).ConfigureAwait(false);

            var itemsOnPage = await catalogContext.Product
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip(req.PageSize * req.PageIndex)
                .Take(req.PageSize)
                .ToListAsync(ct)
                .ConfigureAwait(false);

            var model = new PaginatedItemsViewModel<Product>(req.PageIndex, req.PageSize, totalItem, itemsOnPage);

            await SendAsync(model, 200, ct).ConfigureAwait(false);
        }
    }
}
