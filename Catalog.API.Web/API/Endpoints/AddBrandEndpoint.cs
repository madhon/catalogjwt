namespace Catalog.API.Web.API.Endpoints
{
    using Catalog.API.Application.Abstractions;
    using Catalog.API.Web.API.Endpoints.Requests;
    using Catalog.API.Web.API.Mappers;

    public sealed class AddBrandEndpoint : Endpoint<AddBrandRequest>
    {
        private readonly ICatalogDbContext catalogContext;

        public AddBrandEndpoint(ICatalogDbContext context)
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
            var mapper = new BrandMapper();
            var item = mapper.MapAddBrandRequestToBrand(req);

            catalogContext.Brands.Add(item);
            await catalogContext.SaveChangesAsync(ct).ConfigureAwait(false);
            await SendOkAsync(ct).ConfigureAwait(false);
        }
    }
}
