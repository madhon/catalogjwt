namespace Catalog.Auth;

using System.Threading.RateLimiting;

internal static class RateLimitExtensions
{
	internal static IServiceCollection AddMyRateLimter(this IServiceCollection services)
	{
		services.AddRateLimiter(opts =>
		{
			opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

			opts.AddPolicy(RateLimiterPolicies.RlPoicy, context => RateLimitPartition.GetFixedWindowLimiter(
				partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
				factory: partition => new FixedWindowRateLimiterOptions
				{
					AutoReplenishment = true,
					PermitLimit = 5,
					QueueLimit = 0,
					QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
					Window = TimeSpan.FromMinutes(1)
				}));
		});

		return services;
	}

	internal static IApplicationBuilder UseMyRateLimiter(this IApplicationBuilder app)
	{
		app.UseRateLimiter();
		return app;
	}
}