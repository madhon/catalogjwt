﻿namespace Catalog.API.Web.API;

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Catalog.API.Web.API.Validators;
using FluentValidation;
using ZiggyCreatures.Caching.Fusion;

internal static class ApiStartup
{
	public static IServiceCollection AddMyApi(this IServiceCollection services)
	{
		ArgumentNullException.ThrowIfNull(services);

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
			})
			.AsHybridCache();

		services.AddValidatorsFromAssemblyContaining<AddBrandValidator>();

		services.AddMediator(opts => opts.ServiceLifetime = ServiceLifetime.Scoped);

		services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));

		return services;
	}

	public static void UseMyApi(this WebApplication app)
	{
		app.UseForwardedHeaders();

		app.UseRateLimiter();

		app.UseHeaderPropagation();

		app.MapDefaultEndpoints();

		app.MapGroup("api/v1/catalog/")
			.MapCatalogApi();
	}
}