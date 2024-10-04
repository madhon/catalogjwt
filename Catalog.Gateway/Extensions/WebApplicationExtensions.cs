namespace Catalog.Gateway.Extensions;

public static class WebApplicationExtensions
{
    public static void ConfigureApplication(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.AddDefaultSecurityHeaders();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapReverseProxy();
        app.MapPrometheusScrapingEndpoint();
    }
}