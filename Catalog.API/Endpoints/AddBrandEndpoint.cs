namespace Catalog.API.Endpoints
{
    public sealed class AddBrandEndpoint : Endpoint<AddBrandRequest>
    {
        private readonly CatalogContext catalogContext;

        public AddBrandEndpoint(CatalogContext context)
        {
            this.catalogContext = context;
        }

        public override void Configure()
        {
            Version(1);
            Post("catalog/addBrand");
        }

        public override async Task HandleAsync(AddBrandRequest req, CancellationToken ct)
        {
            var item = req.Adapt<Brand>();

            catalogContext.Brand.Add(item);
            await catalogContext.SaveChangesAsync(ct).ConfigureAwait(false);
            await SendOkAsync(ct).ConfigureAwait(false);
        }
    }
}
