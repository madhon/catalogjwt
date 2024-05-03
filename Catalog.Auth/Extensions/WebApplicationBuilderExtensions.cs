namespace Catalog.Auth;

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

		services.ConfigureHttpJsonOptions(options =>
		{
			options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
		});

		services.AddExceptionHandler<GlobalExceptionHandler>();
		services.AddProblemDetails();
		
		services.AddAuthorization();
		services.AddJwtAuth(configuration);

		services.AddAuthDbContext(configuration);

		services.AddDataProtection()
			.PersistKeysToDbContext<AuthDbContext>();

		services.AddScoped<IPasswordHasher<ApplicationUser>, Argon2PasswordHasher<ApplicationUser>>();

		services.AddValidatorsFromAssemblyContaining<Program>();

		services.Configure<IdentityOptions>(options =>
		{
			options.SignIn.RequireConfirmedAccount = false;
			options.User.RequireUniqueEmail = true;
			options.Password.RequireDigit = true;
			options.Password.RequiredLength = 6;
			options.Password.RequireLowercase = true;
			options.Password.RequireNonAlphanumeric = true;
			options.Password.RequireUppercase = true;
			options.Password.RequiredUniqueChars = 1;
		});

		services.AddIdentity<ApplicationUser, IdentityRole>()
			.AddEntityFrameworkStores<AuthDbContext>()
			.AddDefaultTokenProviders();

		services.AddScoped<IJwtTokenService, JwtTokenService>();
		services.AddScoped<IAuthenticationService, AuthenticationService>();

		services.AddSingleton<ApiMetrics>();
		
		services.AddHealthChecks()
			.AddDbContextCheck<AuthDbContext>();

		services.AddHttpContextAccessor();
		services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));

		services.AddMyRateLimter();
	}
}