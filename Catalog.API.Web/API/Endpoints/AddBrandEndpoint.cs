namespace Catalog.API.Web.API.Endpoints;

using Catalog.API.Application.Abstractions;
using Catalog.API.Web.API.Endpoints.Requests;
using Catalog.API.Web.API.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;

public static class AddBrandEndpoint
{
    public static IEndpointRouteBuilder MapAddBrandEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/catalog/addBrand",
            async Task<Results<Ok, ProblemHttpResult, UnauthorizedHttpResult>>
                (
                    AddBrandRequest request,
                    ICatalogDbContext catalogContext,
                    CancellationToken ct
                )
                =>
            {
                var mapper = new BrandMapper();
                var item = mapper.MapAddBrandRequestToBrand(request);

                catalogContext.Brands.Add(item);
                await catalogContext.SaveChangesAsync(ct).ConfigureAwait(false);

                return TypedResults.Ok();
            })
            .WithName("brands.add")
            .WithTags("brands")
            .Produces<Ok>()
            .Produces<UnauthorizedHttpResult>()
            .ProducesProblemDetails()
            .WithOpenApi()
            .RequireAuthorization();

        return app;
    }
    
}