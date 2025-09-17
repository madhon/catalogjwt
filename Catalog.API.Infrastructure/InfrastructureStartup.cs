namespace Catalog.API.Infrastructure
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class InfrastructureStartup
    {
        public static IServiceCollection AddMyInfrastructureDependencies(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            Authentication.Startup.ConfigureServices(services);
            Persistence.Startup.ConfigureServices(services);

            return services;
        }

        public static IApplicationBuilder UseMyInfrastructure(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app);

            Authentication.Startup.Configure(app);

            return app;
        }
    }
}
