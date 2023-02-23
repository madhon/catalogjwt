namespace Catalog.API
{
    using System.Text;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.HttpOverrides;

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

            var jwtOpts = new JwtOptions();
            configuration.Bind(JwtOptions.Jwt, jwtOpts);
            services.AddSingleton(Options.Create(jwtOpts));

            var secret = jwtOpts.Secret;
            var key = Encoding.ASCII.GetBytes(secret);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
            {
                x.MapInboundClaims = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtOpts.Issuer,
                    ValidAudience = jwtOpts.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddMemoryCache();

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
