namespace Catalog.Auth.Services;

internal interface IJwtTokenService
{
    TokenResult CreateToken(IDictionary<string, object> claims, IEnumerable<string> roles,
        int expiresInMinutes = 120);
}