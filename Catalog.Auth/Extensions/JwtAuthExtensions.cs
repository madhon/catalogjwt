namespace Catalog.Auth.Extensions;

internal static class JwtAuthExtensions
{
    internal static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.Jwt);

        services.AddSingleton<IValidateOptions<JwtOptions>, ValidateJwtOptions>();

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

        services.AddSingleton<EcdsaJwtKeyMaterial>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<JwtOptions>>().Value;
            return new EcdsaJwtKeyMaterial(opts.PrivateKeyPem, isPrivate: true);
        });

        return services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, null!).Services;
    }
}