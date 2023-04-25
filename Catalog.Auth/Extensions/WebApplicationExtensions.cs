namespace Catalog.Auth
{

    public static class WebApplicationExtensions
    {
        public static void ConfigureApplication(this WebApplication app)
        {
            app.UseForwardedHeaders();

            app.UseExceptionHandler();
            app.UseStatusCodePages();
            
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(
                    options =>
                    {

                        foreach (var description in app.DescribeApiVersions())
                        {
                            var url = $"/swagger/{description.GroupName}/swagger.json";
                            var name = description.GroupName.ToUpperInvariant();
                            options.SwaggerEndpoint(url, name);
                        }
                    });

                app.UseDeveloperExceptionPage();
            }
            
            app.UseHeaderPropagation();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/startup");
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions { Predicate = _ => false });
                endpoints.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = _ => false });
                endpoints.MapPrometheusScrapingEndpoint();
                endpoints.MapControllers();
            });
        }
    }
}
