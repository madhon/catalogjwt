namespace Catalog.Gateway.Extensions;

using Microsoft.Extensions.Options;

internal static class WebApplicationBuilderExtensions
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {

        builder.Services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.Jwt);

        builder.Services.AddSingleton<EcdsaJwtKeyMaterial>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<JwtOptions>>().Value;
            return new EcdsaJwtKeyMaterial(opts.PublicKeyPem, isPrivate: false);
        });

        builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOptions: null!);


        builder.Configuration.AddJsonFile("yarp.json", optional: false, reloadOnChange: true);
        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    }

}