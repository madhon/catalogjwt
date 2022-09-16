namespace Catalog.Auth.Services
{
    public interface IAuthenticate
    {
        IEnumerable<Claim>? GetClaims(string token);
        string CreateToken(ClaimsIdentity claimsIdentity, int expiresInMinutes = 30);
    }
}