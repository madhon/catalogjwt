namespace Catalog.API.Infrastructure
{
    using Catalog.API.Application.Abstractions;
    using Catalog.API.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CatalogContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionString"], sqlOpts =>
                {
                    sqlOpts.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            });

            services.AddScoped<ICatalogDbContext, CatalogContext>();
            //services.AddScoped<ICatalogDbContext>(provider => provider.GetRequiredService<CatalogContext>()!);

            return services;
        }

    }
}