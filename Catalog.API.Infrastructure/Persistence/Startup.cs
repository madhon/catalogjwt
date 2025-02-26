namespace Catalog.API.Infrastructure.Persistence;

using Catalog.API.Application.Abstractions;
using Catalog.API.Infrastructure.Persistence.CompiledModels;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

internal static class Startup
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment env)
    {
        services.AddOptions<PersistenceOptions>()
            .BindConfiguration(PersistenceOptions.SectionName);

        services.AddSingleton<IValidateOptions<PersistenceOptions>, PersistenceOptionsValidator>();

        var persistenceOptions = configuration
            .GetSection(PersistenceOptions.SectionName)
            .Get<PersistenceOptions>();

        services.AddSingleton<SlowQueryInterceptor>();

        services.AddDbContextPool<CatalogContext>((sp, options) =>
        {
            options.UseModel(CatalogContextModel.Instance);

            options.UseSqlServer(persistenceOptions!.CatalogDb, sqlOpts =>
            {
                sqlOpts.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
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
        });

        services.AddScoped<ICatalogDbContext, CatalogContext>();
    }

    public static void Configure(IApplicationBuilder app, IConfiguration configuration)
    {
        // empty as not currently used
    }
}