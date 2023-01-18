namespace Catalog.API
{
    using System.Text;
    using Catalog.Api;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

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
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtOpts.Issuer,
                    ValidAudience = jwtOpts.Audience,
                };
            });

            builder.Services.AddDbContext<CatalogContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionString"], options =>
                {
                    options.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            });

            services.AddHealthChecks()
                .AddDbContextCheck<CatalogContext>(); 

            services.AddFastEndpoints(o =>
            {
                o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All;
            });

            services.AddSwaggerDoc(shortSchemaNames: true);

            services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));

            builder.AddOpenTelemetry();
        }
    }
}
