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
                ValidateIssuer = true,
                ValidIssuer = jwtOpts.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOpts.Audience,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                ClockSkew = TimeSpan.FromSeconds(30),
            };
        });

        builder.Configuration.AddJsonFile("yarp.json", optional: false, reloadOnChange: true);
        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
    }

}