namespace Catalog.Auth.Services
{
    using System.Security.Claims;

    public interface IAuthenticate
    {
        string SecretKey { get; set; }

        IEnumerable<Claim>? GetClaims(string token, string issuer, string audience);
        string CreateToken(ClaimsIdentity claimsIdentity, string issuer, string audience, int expiresInMinutes = 30);
    }
}