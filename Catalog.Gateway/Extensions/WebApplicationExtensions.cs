namespace Catalog.Gateway
{
    public static class WebApplicationExtensions
    {
        public static void ConfigureApplication(this WebApplication app)
        {
            app.UseSerilogRequestLogging();
            app.MapPrometheusScrapingEndpoint();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();
            app.MapReverseProxy();
        }
    }
}
