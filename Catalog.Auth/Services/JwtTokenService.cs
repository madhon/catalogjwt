namespace Catalog.Auth.Services;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

public class JwtTokenService : IJwtTokenService
{
    private readonly SigningCredentials signingCredentials;
    private readonly string issuer;
    private readonly string audience;

    public JwtTokenService(IOptions<JwtOptions> jwtOptions)
    {
        ArgumentNullException.ThrowIfNull(jwtOptions);

        var signingKeyBytes = Encoding.UTF8.GetBytes(jwtOptions.Value.Secret);
        var signingKey = new SymmetricSecurityKey(signingKeyBytes);
        signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        issuer = jwtOptions.Value.Issuer;
        audience = jwtOptions.Value.Audience;
    }

    public TokenResult CreateToken(IDictionary<string, object> claims, IEnumerable<string> roles, int expiresInMinutes = 30)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var issuedAt = DateTime.UtcNow;

        var claimsIdentity = new ClaimsIdentity(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = issuedAt.AddMinutes(expiresInMinutes),
            SigningCredentials = signingCredentials,
            Claims = claims,
            Subject = claimsIdentity,
        };

        var expiresIn = TimeSpan.FromMinutes(expiresInMinutes);

        return new TokenResult
        {
            Token = tokenHandler.CreateToken(tokenDescriptor),
            ExpiresIn = Convert.ToInt32(expiresIn.TotalSeconds),
        };
    }
}