namespace Catalog.API.Application
{
    using Catalog.API.Application.Behaviours;
    using Mediator;

    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

            return services;
        }

    }
}
