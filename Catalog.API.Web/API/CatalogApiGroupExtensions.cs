namespace Catalog.API.Web.API;

using Catalog.API.Web.API.Endpoints;

internal static class CatalogApiGroupExtensions
{
    internal static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        app.MapAddBrandEndpoint();
        app.MapProductsEndpoint();
        app.MapAddProductEndpoint();
        return app;
    }
}