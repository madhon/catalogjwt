namespace Catalog.Auth
{

    public static class WebApplicationBuilderExtensions
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;
            var environment = builder.Environment;

            services.AddControllers();

            services.AddFluentValidationAutoValidation(config =>
            {
                config.DisableDataAnnotationsValidation = true;
            });

            services.AddValidatorsFromAssemblyContaining<Program>();

            services.AddEndpointsApiExplorer();

            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.Jwt));
            services.Configure<ArgonOptions>(configuration.GetSection(ArgonOptions.Argon));

            var secret = configuration["jwt:secret"];
            var key = Encoding.ASCII.GetBytes(secret);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["jwt:issuer"],
                    ValidAudience = configuration["jwt:audience"]
                };
            });

            services.AddSwaggerGen();

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
            services.AddScoped<IAuthenticate, Authenticate>();
            services.AddScoped<IAuth, Services.Auth>();

            services.AddHealthChecks()
                .AddDbContextCheck<AuthContext>();

            services.AddHttpContextAccessor();
            services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));
            services.AddOpenTelemetry(builder.Environment);
        }

    }
}
