namespace Catalog.Auth.Services
{
    public interface IAuthenticationService
    {
        string? Authenticate(string email, string password, bool hashPassword = true);
        int? GetUserFromToken(string token);
        string? GetRoleFromToken(string token);
    }
}
