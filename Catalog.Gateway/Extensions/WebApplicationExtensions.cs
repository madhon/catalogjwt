namespace Catalog.Gateway
{
    public static class WebApplicationExtensions
    {
        public static void ConfigureApplication(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();
            app.MapReverseProxy();
        }
    }
}
