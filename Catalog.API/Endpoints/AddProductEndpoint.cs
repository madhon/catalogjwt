namespace Catalog.API.Endpoints
{
    using Catalog.API.Endpoints.Requests;
    using Catalog.API.Infrastructure;
    using Catalog.API.Model;
    using Mapster;

    public class AddProductEndpoint : Endpoint<AddProductRequest>
    {
        private readonly CatalogContext catalogContext;

        public AddProductEndpoint(CatalogContext context)
        {
            this.catalogContext = context;
        }

        public override void Configure()
        {
            Post("addProduct");
        }

        public override async Task HandleAsync(AddProductRequest req, CancellationToken ct)
        {
            var item = req.Adapt<Product>();
            item.Brand = null;

            catalogContext.Product.Add(item);
            await catalogContext.SaveChangesAsync(ct).ConfigureAwait(false);
            await SendOkAsync(ct).ConfigureAwait(false);
        }
    }
}
