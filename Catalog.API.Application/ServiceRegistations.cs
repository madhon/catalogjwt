namespace Catalog.API.Application;

using System.Threading.Channels;
using Catalog.API.Application.Behaviours;
using Catalog.API.Application.Features.Products;
using Mediator;

public static class ServiceRegistations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<AddProductChannelProcessor>();

        services.AddSingleton<Channel<Product>>(
            _=> Channel.CreateUnbounded<Product>(new UnboundedChannelOptions
            {
                SingleReader = true,
                AllowSynchronousContinuations = false,
            }));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FusionCacheBehaviour<,>));

        services.AddSingleton<TimeProvider>(TimeProvider.System);

        return services;
    }
}