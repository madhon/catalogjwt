namespace Catalog.API.Application;

using Catalog.API.Application.Behaviours;
using Mediator;

public static class ServiceRegistations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FusionCacheBehaviour<,>));

        services.AddSingleton<TimeProvider>(TimeProvider.System);

        return services;
    }
}