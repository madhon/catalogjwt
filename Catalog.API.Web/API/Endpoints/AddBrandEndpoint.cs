namespace Catalog.API.Web.API.Endpoints;

public static class AddBrandEndpoint
{
    public static IEndpointRouteBuilder MapAddBrandEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("addBrand",
            async Task<Results<Ok, ProblemHttpResult, UnauthorizedHttpResult, ValidationProblem>>
                (
                    AddBrandRequest request,
                    ICatalogDbContext catalogContext,
                    IValidator<AddBrandRequest> validator,
                    CancellationToken ct
                )
                =>
            {
                var validationResult = await validator.ValidateAsync(request).ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    return TypedResults.ValidationProblem(validationResult.ToDictionary());
                }
                
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
            .ProducesProblem(400)
            .WithOpenApi()
            .RequireAuthorization();

        return app;
    }
    
}