namespace Catalog.Auth.Services
{
    public interface IJwtTokenService
    {
        TokenResult CreateToken(IDictionary<string, object> claims, int expiresInMinutes = 30);
    }
}
