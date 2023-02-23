namespace Catalog.API.Endpoints
{
    using Microsoft.Extensions.Caching.Memory;

    public sealed class ProductsEndpoint : Endpoint<ProductsRequest>
    {
        private readonly IMemoryCache cache;
        private readonly CatalogContext catalogContext;

        public ProductsEndpoint(IMemoryCache cache, CatalogContext context)
        {
            this.cache = cache;
            this.catalogContext = context;
        }

        public override void Configure()
        {
            Version(1);
            Get("catalog/products/{pageSize}/{PageIndex}");
            Summary(s =>
            {
                s.Summary = "Retrieves paged products";
                s.ExampleRequest = new ProductsRequest
                {
                    PageIndex = 0,
                    PageSize = 10
                };
            });
            Roles("read");
        }

        public override async Task HandleAsync(ProductsRequest req, CancellationToken ct)
        {
            var n = User.Id();
            var x = User.Azp();

            string cacheKey = $"products-all-{req.PageIndex}-{req.PageSize}";

            PaginatedItemsViewModel<Product> model = new PaginatedItemsViewModel<Product>(0, 0, 0, new List<Product>());

            model = await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

                var totalItem = await catalogContext.Product
                    .AsNoTracking()
                    .LongCountAsync(ct).ConfigureAwait(false);

                var itemsOnPage = await catalogContext.Product
                    .AsNoTracking()
                    .Where(x =>
                        catalogContext.Product
                            .OrderBy(c => c.Name)
                            .Select(y => y.Id)
                            .Skip(req.PageSize * req.PageIndex)
                            .Take(req.PageSize)
                            .Contains(x.Id))
                    .ToListAsync(ct)
                    .ConfigureAwait(false);

                return new PaginatedItemsViewModel<Product>(req.PageIndex, req.PageSize, totalItem, itemsOnPage);

            }).ConfigureAwait(false);

            await SendAsync(model, 200, ct).ConfigureAwait(false);
        }
    }
}
