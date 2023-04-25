namespace Catalog.API.Web.API
{
    using FastExpressionCompiler;
    using Microsoft.AspNetCore.HttpOverrides;
    using ZiggyCreatures.Caching.Fusion;

    public static class ApiStartup
    {
        public static void AddMyApi(this IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(opts =>
            {
                opts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                opts.KnownNetworks.Clear();
                opts.KnownProxies.Clear();
            });

            services.AddHealthChecks();

            services.AddHttpContextAccessor();

            services.AddMemoryCache();
            services.AddFusionCache()
                .WithOptions(opts =>
                {
                    opts.DefaultEntryOptions = new FusionCacheEntryOptions { Duration = TimeSpan.FromMinutes(2) };
                });

            services.AddMediator(opts => opts.ServiceLifetime = ServiceLifetime.Scoped);

            services.AddFastEndpoints(o =>
            {
                o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All;
            });

            services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));

        }

        public static void UseMyApi(this IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment environment)
        {
            TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.CompileFast();

            app.UseForwardedHeaders();

            app.UseFastEndpoints(c =>
            {
                c.Endpoints.ShortNames = true;
                c.Endpoints.RoutePrefix = "api";
                c.Versioning.Prefix = "v";
                c.Versioning.DefaultVersion = 1;
                c.Versioning.PrependToRoute = true;
            });

            app.UseHeaderPropagation();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/startup");
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions { Predicate = _ => false });
                endpoints.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = _ => false });
                endpoints.MapPrometheusScrapingEndpoint();
            });

            
        }
    }
}
