namespace Catalog.API.Infrastructure
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class InfrastructureStartup
    {
        public static void AddMyInfrastructureDependencies(this IServiceCollection services,
            IConfiguration configuration, IHostEnvironment environment)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(environment);

            Authentication.Startup.ConfigureServices(services, configuration);
            Persistence.Startup.ConfigureServices(services, configuration, environment);
        }

        public static void UseMyInfrastructure(this IApplicationBuilder app, IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(environment);

            Authentication.Startup.Configure(app);
            Persistence.Startup.Configure(app, configuration);
        }
    }
}
