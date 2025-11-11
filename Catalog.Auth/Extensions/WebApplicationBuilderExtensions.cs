namespace Catalog.Auth;

using Catalog.Auth.Login;
using Catalog.Auth.Signup;
using Microsoft.AspNetCore.HttpOverrides;

internal static class WebApplicationBuilderExtensions
{
#pragma warning disable MA0051
	public static IHostApplicationBuilder RegisterServices(this IHostApplicationBuilder builder)
#pragma warning restore MA0051
	{
		ArgumentNullException.ThrowIfNull(builder);

		var services = builder.Services;
		var configuration = builder.Configuration;

		services.Configure<ForwardedHeadersOptions>(opts =>
		{
			opts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
			opts.KnownIPNetworks.Clear();
			opts.KnownProxies.Clear();
		});

		services.ConfigureHttpJsonOptions(options =>
		{
			options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
		});

		services.AddSingleton(TimeProvider.System);

		services.AddExceptionHandler<GlobalExceptionHandler>();
		services.AddProblemDetails();

		services.AddAuthorization();
		services.AddJwtAuth(configuration);

		services.AddAuthDbContext(configuration);

		services.AddDataProtection()
			.PersistKeysToDbContext<AuthDbContext>();

		services.AddScoped<IPasswordHasher<ApplicationUser>, Argon2PasswordHasher<ApplicationUser>>();

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

		services.AutoRegisterFromCatalogAuth();

		services.AddValidators();

		services.AddHealthChecks()
			.AddDbContextCheck<AuthDbContext>();

		services.AddHttpContextAccessor();
		services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));

		services.AddMyRateLimter();

		return builder;
	}
}