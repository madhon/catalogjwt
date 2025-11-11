namespace Catalog.API.Web.API.Endpoints;

using System.Threading.Channels;
using Catalog.API.Application.Features.Products;

internal static class AddProductEndpoint
{
    public static IEndpointRouteBuilder MapAddProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("addProduct",
                async Task<Results<Ok, ProblemHttpResult, UnauthorizedHttpResult>>
                    (
                        AddProductRequest request,
                        Channel<Product> productChannel,
                        CancellationToken ct
                    )
                    =>
                {
                    var mapper = new ProductMapper();
                    var item = mapper.MapAddProductRequestToProduct(request);
                    item.Brand = null;

                    await productChannel.Writer.WriteAsync(item, ct);

                    return TypedResults.Ok();
                })
            .WithName("products.add")
            .WithTags("products")
            .Produces<Ok>()
            .Produces<UnauthorizedHttpResult>()
            .ProducesProblem(400)
            .RequireAuthorization();

        return app;
    }
}
