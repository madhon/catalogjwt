namespace Catalog.API
{
    using Microsoft.AspNetCore.HttpOverrides;
    using ZiggyCreatures.Caching.Fusion;

    public static class WebApplicationBuilderExtensions
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            services.Configure<ForwardedHeadersOptions>(opts =>
            {
                opts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                opts.KnownNetworks.Clear();
                opts.KnownProxies.Clear();
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddMemoryCache();
            builder.Services.AddFusionCache(opts =>
            {
                opts.DefaultEntryOptions = new FusionCacheEntryOptions { Duration = TimeSpan.FromMinutes(2) };
            });

            builder.Services.AddDbContext<CatalogContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionString"], sqlOpts =>
                {
                    sqlOpts.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            });

            services.AddHealthChecks()
                .AddDbContextCheck<CatalogContext>(); 

            services.AddFastEndpoints(o =>
            {
                o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All;
            });

            services.AddSwaggerDoc(maxEndpointVersion: 1, settings: s =>
            {
                s.DocumentName = "v1.0";
                s.Title = "Catalog API";
                s.Version = "v1.0";
            },  shortSchemaNames: true);

            services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));

        }
    }
}
