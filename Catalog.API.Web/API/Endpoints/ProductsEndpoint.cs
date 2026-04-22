namespace Catalog.API.Web.API.Endpoints;

using Catalog.API.Application.Features.ListProducts;
using Catalog.API.Web.API.ViewModel;
using Mediator;
using Microsoft.AspNetCore.Authorization;

internal static class ProductsEndpoint
{
	public static IEndpointRouteBuilder MapProductsEndpoint(this IEndpointRouteBuilder app)
	{
		app.MapGet("products/{pageSize:int}/{pageIndex:int}", [Authorize(Roles = "read")]
				async Task<Results<Ok<PaginatedItemsViewModel<Product>>, ProblemHttpResult, UnauthorizedHttpResult>>
					(
						int pageSize,
						int pageIndex,
						IMediator mediator,
						CancellationToken ct
					)
					=>
				{
                if (pageSize <= 0 || pageIndex < 0)
                {
                        return TypedResults.Problem(
                            detail: "pageSize must be > 0 and pageIndex must be >= 0",
                            statusCode: 400,
                            title: "Invalid paging parameters");
                }
                var response = await mediator.Send(new ListProductsRequest(pageIndex, pageSize), ct); 
                var model = new PaginatedItemsViewModel<Product>(pageIndex, pageSize, response.TotalItems, response.Items);
                return TypedResults.Ok(model);
				})
			.WithName("products.get")
			.WithTags("products")
			.Produces<PaginatedItemsViewModel<Product>>(StatusCodes.Status200OK, "application/json")
			.Produces<UnauthorizedHttpResult>()
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.RequireAuthorization();

		return app;
	}
}