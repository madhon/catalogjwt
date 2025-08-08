namespace Catalog.API.Web.API.Endpoints;

using System.Globalization;
using Catalog.API.Application.Features.ListProducts;
using Catalog.API.Web.API.ViewModel;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Hybrid;
using ZiggyCreatures.Caching.Fusion;

internal static class ProductsEndpoint
{
	public static IEndpointRouteBuilder MapProductsEndpoint(this IEndpointRouteBuilder app)
	{
		app.MapGet("products/{pageSize:int}/{pageIndex:int}", [Authorize(Roles = "read")]
				async Task<Results<Ok<PaginatedItemsViewModel<Product>>, ProblemHttpResult, UnauthorizedHttpResult>>
					(
						int pageSize,
						int pageIndex,
						HybridCache cache,
						IMediator mediator,
						CancellationToken ct
					)
					=>
				{
					var response = await mediator.Send(new ListProductsRequest(pageIndex, pageSize), ct).ConfigureAwait(false);
					var model = new PaginatedItemsViewModel<Product>(pageIndex, pageSize, response.TotalItems, response.Items);
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