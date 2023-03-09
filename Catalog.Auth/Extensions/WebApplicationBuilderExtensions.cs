namespace Catalog.Auth
{
    using Microsoft.AspNetCore.Identity;
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

            services.AddSwaggerDoc(maxEndpointVersion: 1, settings: s =>
            {
                s.DocumentName = "v1.0";
                s.Title = "Auth API";
                s.Version = "v1.0";
            }, shortSchemaNames: true);


            var jwtOpts = new JwtOptions();
            configuration.Bind(JwtOptions.Jwt, jwtOpts);
            services.AddSingleton(Options.Create(jwtOpts));



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

            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionString"], options =>
                {
                    options.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                    options.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            });

            services.AddDataProtection()
                .PersistKeysToDbContext<AuthDbContext>();

            services.AddScoped<IPasswordHasher<ApplicationUser>, Argon2PasswordHasher<ApplicationUser>>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredUniqueChars = 1;
                }).AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();
            

            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddHealthChecks()
                .AddDbContextCheck<AuthDbContext>();

            services.AddHttpContextAccessor();
            services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));
        }
    }
}
