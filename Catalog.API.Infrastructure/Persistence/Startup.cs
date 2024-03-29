﻿namespace Catalog.API.Infrastructure.Persistence;

using Catalog.API.Application.Abstractions;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal static class Startup
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment env)
    {
        services.AddDbContextPool<CatalogContext>(options =>
        {
            options.UseModel(MyCompiledModels.CatalogContextModel.Instance);
            options.UseSqlServer(configuration["ConnectionString"], sqlOpts =>
            {
                sqlOpts.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            });

            options.UseExceptionProcessor();
        });

        services.AddScoped<ICatalogDbContext, CatalogContext>();

    }

    public static void Configure(IApplicationBuilder app, IConfiguration configuration)
    {
        // empty as not currently used
    }
}