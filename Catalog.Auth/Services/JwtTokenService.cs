namespace Catalog.Auth.Services;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

[RegisterSingleton]
internal sealed class JwtTokenService : IJwtTokenService
{
    private readonly JsonWebTokenHandler tokenHandler = new();
    private readonly SigningCredentials signingCredentials;
    private readonly string issuer;
    private readonly string audience;

    private readonly TimeProvider timeProvider;

    public JwtTokenService(IOptions<JwtOptions> jwtOptions, EcdsaJwtKeyMaterial ecdsaJwtKeyMaterial, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(jwtOptions);
        ArgumentNullException.ThrowIfNull(ecdsaJwtKeyMaterial);
        ArgumentNullException.ThrowIfNull(timeProvider);

        var options = jwtOptions.Value ?? throw new ArgumentException("JwtOptions.Value must not be null.", nameof(jwtOptions));

        signingCredentials = ecdsaJwtKeyMaterial.SigningCredentials!;
        issuer = options.Issuer;
        audience = options.Audience;
        this.timeProvider = timeProvider;
    }

    public TokenResult CreateToken(IDictionary<string, object> claims, IEnumerable<string> roles, int expiresInMinutes)
    {
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(roles);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(expiresInMinutes, 0);

        var issuedAt = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = issuedAt.AddMinutes(expiresInMinutes);

        var claimsIdentity = new ClaimsIdentity(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            TokenType = "at+jwt",
            Issuer = issuer,
            Audience = audience,
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = expiresAt,
            SigningCredentials = signingCredentials,
            Claims = new Dictionary<string, object>(claims, StringComparer.OrdinalIgnoreCase),
            Subject = claimsIdentity,
        };

        return new TokenResult
        {
            Token = tokenHandler.CreateToken(tokenDescriptor),
            ExpiresIn = checked(expiresInMinutes * 60),
        };
    }
}