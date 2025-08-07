namespace Catalog.Gateway.Extensions;

using Microsoft.Extensions.Options;

internal static class WebApplicationBuilderExtensions
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        var jwtOpts = new JwtOptions();
        builder.Configuration.Bind(JwtOptions.Jwt, jwtOpts);
        builder.Services.AddSingleton(Options.Create(jwtOpts));

        var secret = jwtOpts.Secret;
        var key = Encoding.ASCII.GetBytes(secret);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtOpts.Issuer,
                ValidAudience = jwtOpts.Audience,
            };
        });

        builder.Configuration.AddJsonFile("yarp.json", optional: false, reloadOnChange: true);
        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    }

}