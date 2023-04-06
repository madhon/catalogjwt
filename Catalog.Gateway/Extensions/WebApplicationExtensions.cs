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
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapReverseProxy();
                endpoints.MapPrometheusScrapingEndpoint();
            });
        }
    }
}
