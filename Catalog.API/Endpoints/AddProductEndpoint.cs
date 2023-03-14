namespace Catalog.API.Endpoints
{
    using Catalog.API.Application.Common;
    using Catalog.API.Domain.Entities;
    using Catalog.API.Endpoints.Requests;
    using Mapster;

    public sealed class AddProductEndpoint : Endpoint<AddProductRequest>
    {
        private readonly ICatalogDbContext catalogContext;

        public AddProductEndpoint(ICatalogDbContext context)
        {
            this.catalogContext = context;
        }

        public override void Configure()
        {
            Version(1);
            Post("catalog/addProduct");
        }

        public override async Task HandleAsync(AddProductRequest req, CancellationToken ct)
        {
            var item = req.Adapt<Product>();
            item.Brand = null;

            catalogContext.Products.Add(item);
            await catalogContext.SaveChangesAsync(ct).ConfigureAwait(false);
            await SendOkAsync(ct).ConfigureAwait(false);
        }
    }
}
