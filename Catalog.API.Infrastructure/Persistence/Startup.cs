namespace Catalog.API.Infrastructure.Persistence;

using Catalog.API.Application.Abstractions;
using Catalog.API.Infrastructure.Persistence.CompiledModels;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

internal static class Startup
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddOptions<PersistenceOptions>()
            .BindConfiguration(PersistenceOptions.SectionName);

        services.AddSingleton<IValidateOptions<PersistenceOptions>, PersistenceOptionsValidator>();

        services.AddSingleton<SlowQueryInterceptor>();

        services.AddDbContextPool<CatalogContext>((sp, options) =>
        {
            var persistenceOptions = sp.GetRequiredService<IOptions<PersistenceOptions>>().Value;

            options.UseModel(CatalogContextModel.Instance);
            options.UseSqlServer(persistenceOptions.CatalogDb, sqlOpts =>
            {
                sqlOpts.EnableRetryOnFailure(
                    maxRetryCount: 15,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            options.AddInterceptors(sp.GetRequiredService<SlowQueryInterceptor>());

            options.UseExceptionProcessor();

            if (persistenceOptions.EnableDetailedErrors)
            {
                options.EnableDetailedErrors();
            }

            if (persistenceOptions.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        }, poolSize: 128);

        services.AddScoped<ICatalogDbContext, CatalogContext>();
    }
}