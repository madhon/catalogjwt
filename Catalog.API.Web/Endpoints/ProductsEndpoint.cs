namespace Catalog.API.Endpoints
{
    using Catalog.API.Application.Features.ListProducts;
    using Catalog.API.Domain.Entities;
    using Mediator;
    using ZiggyCreatures.Caching.Fusion;

    public sealed class ProductsEndpoint : Endpoint<ProductsRequest>
    {
        private readonly IFusionCache cache;
        private readonly IMediator mediator;

        public ProductsEndpoint(IFusionCache cache, IMediator mediator)
        {
            this.cache = cache;
            this.mediator = mediator;
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
                var response = await mediator.Send(new ListProductsRequest(req.PageIndex, req.PageSize), ct);

                return new PaginatedItemsViewModel<Product>(req.PageIndex, req.PageSize, response.TotalItems, response.Items);

            }, token: ct).ConfigureAwait(false);

            await SendAsync(model, 200, ct).ConfigureAwait(false);
        }
    }
}
