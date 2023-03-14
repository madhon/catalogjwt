﻿namespace Catalog.API.Endpoints
{
    using Catalog.API.Application.Common;
    using Catalog.API.Domain.Entities;
    using ZiggyCreatures.Caching.Fusion;

    public sealed class ProductsEndpoint : Endpoint<ProductsRequest>
    {
        private readonly IFusionCache cache;
        private readonly ICatalogDbContext catalogContext;

        public ProductsEndpoint(IFusionCache cache, ICatalogDbContext context)
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
            //var n = User.Id();
            //var x = User.Azp();

            var cacheKey = $"products-all-{req.PageIndex}-{req.PageSize}";
            var model = new PaginatedItemsViewModel<Product>(0, 0, 0, new List<Product>());
            
            model = await cache.GetOrSetAsync(cacheKey, async _ =>
            {

                var totalItem = await catalogContext.Products
                    .AsNoTracking()
                    .LongCountAsync(ct).ConfigureAwait(false);

                var itemsOnPage = await catalogContext.Products
                    .AsNoTracking()
                    .Where(x =>
                        catalogContext.Products
                            .OrderBy(c => c.Name)
                            .Select(y => y.Id)
                            .Skip(req.PageSize * req.PageIndex)
                            .Take(req.PageSize)
                            .Contains(x.Id))
                    .ToListAsync(ct)
                    .ConfigureAwait(false);

                return new PaginatedItemsViewModel<Product>(req.PageIndex, req.PageSize, totalItem, itemsOnPage);

            }, token: ct).ConfigureAwait(false);

            await SendAsync(model, 200, ct).ConfigureAwait(false);
        }
    }
}
