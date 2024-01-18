﻿namespace Catalog.Auth.Extensions;

public static class AuthDbContextExtensions
{
    public static IServiceCollection AddAuthDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseSqlServer(configuration["ConnectionString"], options =>
            {
                options.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                options.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            });
        });

        return services;
    }
}