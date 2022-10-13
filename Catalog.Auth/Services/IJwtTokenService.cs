namespace Catalog.Auth.Services
{
    public interface IJwtTokenService
    {
        IEnumerable<Claim>? GetClaims(string token);
        string CreateToken(ClaimsIdentity claimsIdentity, int expiresInMinutes);
    }
}
