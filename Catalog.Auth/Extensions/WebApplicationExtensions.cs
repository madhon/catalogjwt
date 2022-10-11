namespace Catalog.Auth
{
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;

    public static class WebApplicationExtensions
    {
        public static void ConfigureApplication(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHeaderPropagation();
            app.UseAuthentication();

            app.MapHealthChecks("/health/startup");
            app.MapHealthChecks("/healthz", new HealthCheckOptions { Predicate = _ => false });
            app.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = _ => false });

            app.MapControllers();
        }
    }
}
