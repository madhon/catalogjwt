namespace Catalog.Auth.Services;

public interface IJwtTokenService
{
    TokenResult CreateToken(IDictionary<string, object> claims, IEnumerable<string> roles,
        int expiresInMinutes = 30);
}