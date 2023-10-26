namespace Catalog.API.Web.API.Endpoints;

using Catalog.API.Application.Features.ListProducts;
using Catalog.API.Web.API.ViewModel;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using ZiggyCreatures.Caching.Fusion;

public static class ProductsEndpoint
{

	public static IEndpointRouteBuilder MapProductsEndpoint(this IEndpointRouteBuilder app)
	{
		app.MapGet("api/v1/catalog/products/{pageSize}/{pageIndex}", [Authorize(Roles = "read")]
				async Task<Results<Ok<PaginatedItemsViewModel<Product>>, ProblemHttpResult, UnauthorizedHttpResult>>
					(
						int pageSize,
						int pageIndex,
						IFusionCache cache,
						IMediator mediator,
						CancellationToken ct
					)
					=>
				{
					var cacheKey = $"products-all-{pageIndex}-{pageSize}";

					var model = await cache.GetOrSetAsync(cacheKey, async _ =>
					{
						var response = await mediator.Send(new ListProductsRequest(pageIndex, pageSize), ct);

						return new PaginatedItemsViewModel<Product>(pageIndex, pageSize, response.TotalItems, response.Items);

					}, token: ct).ConfigureAwait(false);

					return TypedResults.Ok(model);
				})
			.WithName("products.get")
			.WithTags("products")
			.Produces<PaginatedItemsViewModel<Product>>(200, "application/json")
			.Produces<UnauthorizedHttpResult>()
			.ProducesProblem(400)
			.WithOpenApi()
			.RequireAuthorization();

		return app;
	}
}