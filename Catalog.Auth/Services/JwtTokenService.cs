namespace Catalog.Auth.Services;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

[RegisterScoped]
internal sealed class JwtTokenService : IJwtTokenService
{
    private readonly JsonWebTokenHandler tokenHandler = new();
    private readonly SigningCredentials signingCredentials;
    private readonly string issuer;
    private readonly string audience;

    private readonly TimeProvider timeProvider;

    public JwtTokenService(IOptions<JwtOptions> jwtOptions, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(jwtOptions);
        ArgumentNullException.ThrowIfNull(timeProvider);
        var options = jwtOptions.Value ?? throw new ArgumentException("JwtOptions.Value must not be null.", nameof(jwtOptions));

        var signingKeyBytes = Encoding.UTF8.GetBytes(options.Secret);
        if (signingKeyBytes.Length < 32) // recommended minimum for HS256
        {
            throw new ArgumentException("JwtOptions.Secret should be at least 32 bytes for HS256.", nameof(jwtOptions));
        }

        var signingKey = new SymmetricSecurityKey(signingKeyBytes);
        signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        issuer = options.Issuer;
        audience = options.Audience;
        this.timeProvider = timeProvider;
    }

    public TokenResult CreateToken(IDictionary<string, object> claims, IEnumerable<string> roles, int expiresInMinutes = 120)
    {
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(roles);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(expiresInMinutes, 0);

        var issuedAt = timeProvider.GetUtcNow().UtcDateTime;
        var expiresAt = issuedAt.AddMinutes(expiresInMinutes);

        var claimsIdentity = new ClaimsIdentity(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
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