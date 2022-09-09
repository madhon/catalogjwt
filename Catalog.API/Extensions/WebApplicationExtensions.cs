namespace Catalog.API
{
    using FastExpressionCompiler;
    using Mapster;

    public static class WebApplicationExtensions
    {
        public static void ConfigureApplication(this WebApplication app)
        {
            TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.CompileFast();

            app.UseForwardedHeaders();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseFastEndpoints(c =>
            {
                c.Endpoints.ShortNames = true;
                c.Endpoints.RoutePrefix = "api/v1/catalog";
            });


            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.MapHealthChecks("/health/startup");
            app.MapHealthChecks("/healthz", new HealthCheckOptions { Predicate = _ => false });
            app.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = _ => false });
        }

    }
}
