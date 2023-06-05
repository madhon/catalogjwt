namespace Catalog.API.Web.API.Endpoints;

using Catalog.API.Application.Abstractions;
using Catalog.API.Web.API.Endpoints.Requests;
using Catalog.API.Web.API.Mappers;

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
        var mapper = new ProductMapper();
        var item = mapper.MapAddProductRequestToProduct(req);
        
        item.Brand = null;

        catalogContext.Products.Add(item);
        await catalogContext.SaveChangesAsync(ct).ConfigureAwait(false);
        await SendOkAsync(ct).ConfigureAwait(false);
    }
}
