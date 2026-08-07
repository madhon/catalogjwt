namespace Catalog.Gateway.Extensions;

using Microsoft.Extensions.Options;

internal sealed class ConfigureJwtBearerOptions(
    IOptions<JwtOptions> jwtOptions,
    EcdsaJwtKeyMaterial ecdsaJwtKeyMaterial) : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOptions jwtOptions = jwtOptions.Value;

    public void Configure(JwtBearerOptions options) => Configure(string.Empty, options);

    public void Configure(string? name, JwtBearerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (!string.Equals(name, JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
            return;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = ecdsaJwtKeyMaterial.SecurityKey,
            ValidAlgorithms = [SecurityAlgorithms.EcdsaSha256],
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    }
}