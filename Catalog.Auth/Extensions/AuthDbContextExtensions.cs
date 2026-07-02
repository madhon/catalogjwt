namespace Catalog.Auth.Extensions;

internal static class AuthDbContextExtensions
{
    public static IServiceCollection AddAuthDbContext(this IServiceCollection services)
    {
        services.AddOptions<PersistenceOptions>()
            .BindConfiguration(PersistenceOptions.SectionName);

        services.AddSingleton<IValidateOptions<PersistenceOptions>, PersistenceOptionsValidator>();

        services.AddSingleton<SlowQueryInterceptor>();

        services.AddDbContextPool<AuthDbContext>((sp, options) =>
        {
            var persistenceOptions = sp.GetRequiredService<IOptions<PersistenceOptions>>().Value;

            options.UseModel(MyCompiledModels.AuthDbContextModel.Instance);
            options.UseSqlServer(persistenceOptions.AuthDb, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure();
                sqlOptions.UseCompatibilityLevel(160);
            });

            options.AddInterceptors(sp.GetRequiredService<SlowQueryInterceptor>());

            if (persistenceOptions.EnableDetailedErrors)
            {
                options.EnableDetailedErrors();
            }

            if (persistenceOptions.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }
}