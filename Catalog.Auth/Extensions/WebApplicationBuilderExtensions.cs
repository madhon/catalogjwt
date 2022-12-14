namespace Catalog.Auth
{
    using Microsoft.Extensions.Options;

    public static class WebApplicationBuilderExtensions
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            services.AddFastEndpoints(o =>
            {
                o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All;
            });
            
            services.AddSwaggerDoc(shortSchemaNames: true);


            var jwtOpts = new JwtOptions();
            configuration.Bind(JwtOptions.Jwt, jwtOpts);
            services.AddSingleton(Options.Create(jwtOpts));

            var argonOpts = new ArgonOptions();
            configuration.Bind(ArgonOptions.Argon, argonOpts);
            services.AddSingleton(Options.Create(argonOpts));

            var secret = jwtOpts.Secret;
            var key = Encoding.ASCII.GetBytes(secret);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
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

            services.AddDbContext<AuthContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionString"], options =>
                {
                    options.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                    options.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            });

            services.AddDataProtection()
                .PersistKeysToDbContext<AuthContext>();

            services.AddScoped<IArgonService, ArgonService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddHealthChecks()
                .AddDbContextCheck<AuthContext>();

            services.AddHttpContextAccessor();
            services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));
            
            builder.AddOpenTelemetry();
        }
    }
}
