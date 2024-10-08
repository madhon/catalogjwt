namespace Catalog.API.Web.API.Endpoints;

public static class AddProductEndpoint
{
    public static IEndpointRouteBuilder MapAddProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("addProduct",
                async Task<Results<Ok, ProblemHttpResult, UnauthorizedHttpResult>>
                    (
                        AddProductRequest request,
                        ICatalogDbContext catalogContext,
                        CancellationToken ct
                    )
                    =>
                {
                    var mapper = new ProductMapper();
                    var item = mapper.MapAddProductRequestToProduct(request);
                    item.Brand = null;

                    catalogContext.Products.Add(item);
                    await catalogContext.SaveChangesAsync(ct).ConfigureAwait(false);

                    return TypedResults.Ok();
                })
            .WithName("products.add")
            .WithTags("products")
            .Produces<Ok>()
            .Produces<UnauthorizedHttpResult>()
            .ProducesProblem(400)
            .WithOpenApi()
            .RequireAuthorization();

        return app;
    }
}
