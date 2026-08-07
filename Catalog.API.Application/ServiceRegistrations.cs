namespace Catalog.API.Application;

using System.Threading.Channels;
using Catalog.API.Application.Behaviours;
using Catalog.API.Application.Diagnostics;
using Catalog.API.Application.Features.Products;
using Mediator;
using Microsoft.Extensions.Options;

public static class ServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<AddProductChannelProcessor>();

        services.AddSingleton<MediatorMetrics>();

        services.AddSingleton<Channel<Product>>(sp =>
        {
            var capacity = Math.Max(1, sp.GetRequiredService<IOptions<AddProductChannelOptions>>().Value.Capacity);
            return Channel.CreateBounded<Product>(new BoundedChannelOptions(capacity)
            {
                SingleReader = true,
                FullMode = BoundedChannelFullMode.Wait,
                AllowSynchronousContinuations = false,
            });
        });

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FusionCacheBehaviour<,>));

        services.AddSingleton<TimeProvider>(TimeProvider.System);

        return services;
    }
}