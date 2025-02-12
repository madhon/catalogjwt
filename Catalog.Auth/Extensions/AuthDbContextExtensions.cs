namespace Catalog.Auth.Extensions;

public static class AuthDbContextExtensions
{
    public static IServiceCollection AddAuthDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<AuthDbContext>(options =>
        {
            //options.UseModel(MyCompiledModels.AuthDbContextModel.Instance);
            options.LogTo(Console.WriteLine, LogLevel.Information);
            options.UseSqlServer(configuration["ConnectionString"], sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            });
        });

        return services;
    }
}