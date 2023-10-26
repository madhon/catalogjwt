namespace Catalog.API.Web.API;

using Catalog.API.Web.API.Endpoints;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Catalog.API.Web.API.Validators;
using FluentValidation;
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


		services.AddRateLimiter(rlo => {

			rlo.RejectionStatusCode = 429;
			rlo.AddFixedWindowLimiter(policyName: "fixed", options =>
			{
				options.PermitLimit = 4;
				options.Window = TimeSpan.FromSeconds(12);
				options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
				options.QueueLimit = 2;
			});

		});

		services.AddHealthChecks();

		services.AddProblemDetails();
		
		services.AddHttpContextAccessor();

		services.AddMemoryCache();
		services.AddFusionCache()
			.TryWithAutoSetup()
			.WithOptions(opts =>
			{
				opts.DefaultEntryOptions = new FusionCacheEntryOptions { Duration = TimeSpan.FromMinutes(2) };
			});

		services.AddValidatorsFromAssemblyContaining<AddBrandValidator>();
		
		services.AddMediator(opts => opts.ServiceLifetime = ServiceLifetime.Scoped);

		services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));
	}

	public static void UseMyApi(this IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment environment)
	{
		app.UseForwardedHeaders();

		app.UseRateLimiter();

		app.UseHeaderPropagation();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapHealthChecks("/health/startup");
			endpoints.MapHealthChecks("/healthz", new HealthCheckOptions { Predicate = _ => false });
			endpoints.MapHealthChecks("/ready", new HealthCheckOptions { Predicate = _ => false });
			endpoints.MapAddBrandEndpoint();
			endpoints.MapProductsEndpoint();
			endpoints.MapAddProductEndpoint();
			endpoints.MapPrometheusScrapingEndpoint();
		});

			
	}
}