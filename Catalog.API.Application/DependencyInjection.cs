namespace Catalog.API.Application;

using Catalog.API.Application.Behaviours;
using Mediator;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FusionCacheBehaviour<,>));

        return services;
    }
}