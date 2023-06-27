namespace Catalog.Auth
{

	using Catalog.Auth.Login;
    using Catalog.Auth.Signup;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerUI;

    public static class WebApplicationExtensions
    {
        public static void ConfigureApplication(this WebApplication app)
        {
            app.UseForwardedHeaders();

			app.UseExceptionHandler();
            app.UseStatusCodePages();

            app.UseSerilogRequestLogging();

            app.UseRouting();
			
            app.UseMyRateLimiter();

			app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "docs/{documentName}/openapi.json";
                    c.PreSerializeFilters.Add((swagger, httpReq) =>
                    {
                        swagger.Servers = new List<OpenApiServer>()
                        {
                            new OpenApiServer
                                { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{httpReq.PathBase.Value}" }
                        };
                    });

                    app.UseSwaggerUI(
                        options =>
                        {
                            options.RoutePrefix = "docs";
                            options.DocExpansion(DocExpansion.List);
                            options.DisplayRequestDuration();
                            options.DefaultModelExpandDepth(-1);

                            foreach (var description in app.DescribeApiVersions())
                            {
                                var url = $"/docs/{description.GroupName}/openapi.json";
                                var name = description.GroupName.ToUpperInvariant();
                                options.SwaggerEndpoint(url, name);
                            }
                        });

                    app.UseDeveloperExceptionPage();
                });

                app.UseHeaderPropagation();


                var versionSet = app.NewApiVersionSet()
                    .HasApiVersion(new ApiVersion(1.0))
                    .ReportApiVersions()
                    .Build();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("/health/startup");
                    endpoints.MapHealthChecks("/healthz", new HealthCheckOptions { Predicate = _ => false });
                    endpoints.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = _ => false });
                    endpoints.MapPrometheusScrapingEndpoint();
                    endpoints.MapLoginEndpoint(versionSet);
                    endpoints.MapSignUpEndpoint(versionSet);
                });
            }
        }
    }
}
