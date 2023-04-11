namespace Catalog.Gateway
{
    public static class WebApplicationExtensions
    {
        public static void ConfigureApplication(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapReverseProxy();
            app.MapPrometheusScrapingEndpoint();
        }
    }
}
