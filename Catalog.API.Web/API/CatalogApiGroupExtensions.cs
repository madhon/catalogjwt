namespace Catalog.API.Web.API;

using Catalog.API.Web.API.Endpoints;

internal static class CatalogApiGroupExtensions
{
    internal static RouteGroupBuilder MapCatalogApi(this RouteGroupBuilder app)
    {
        app.MapAddBrandEndpoint();
        app.MapProductsEndpoint();
        app.MapAddProductEndpoint();
        return app;
    }
}