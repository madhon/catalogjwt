namespace Catalog.API.Infrastructure.Authentication.Settings;

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

public partial class ConfigureJwtBearerOptions(IOptions<AuthenticationSettings> jwtOptions, EcdsaJwtKeyMaterial ecdsaJwtKeyMaterial, ILogger<ConfigureJwtBearerOptions> logger) : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthenticationSettings jwtOptions = jwtOptions.Value;

    public void Configure(JwtBearerOptions options)
    {
        Configure(string.Empty, options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (!string.Equals(name, JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        options.SaveToken = false;
        // prevent from mapping "sub" claim to nameidentifier.
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

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

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                if (context.AuthenticateFailure != null)
                {
                    LogAuthenticationFailed(context.AuthenticateFailure.Message);
                }

                return Task.CompletedTask;
            },
        };
    }

    [LoggerMessage(EventId = 1001, Level = LogLevel.Error, Message = "Authentication challenge: {message}")]
    private partial void LogAuthenticationFailed(string message);
}