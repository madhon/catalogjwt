namespace Catalog.Auth;

    public static class WebApplicationExtensions
    {
        public static void ConfigureApplication(this WebApplication app)
        {
            app.UseForwardedHeaders();

			app.UseExceptionHandler();
            app.UseStatusCodePages();

            app.UseSerilogRequestLogs();

            app.UseRouting();

            app.UseMyRateLimiter();

			app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerExtension();
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHeaderPropagation();
            
            var versionSet = app.NewApiVersionSet()
                    .HasApiVersion(new ApiVersion(1.0))
                    .ReportApiVersions()
                    .Build();
           
#pragma warning disable ASP0014
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("/health/startup");
                    endpoints.MapHealthChecks("/healthz", new HealthCheckOptions { Predicate = _ => false });
                    endpoints.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = _ => false });
                    endpoints.MapPrometheusScrapingEndpoint();
                    endpoints.MapAuthApi(versionSet);
                });
#pragma warning restore ASP0014
            }
        }


