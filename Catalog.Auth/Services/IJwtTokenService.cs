namespace Catalog.Auth.Services
{
    public interface IJwtTokenService
    {
        IEnumerable<Claim>? GetClaims(string token);
        TokenResult CreateToken(ClaimsIdentity claimsIdentity, IDictionary<string, object> additionalClaims, int expiresInMinutes = 30);
    }
}
