namespace Catalog.Auth.Services
{
    public interface IAuthenticationService
    {
        Task CreateUser(string email, string password, string fullName, CancellationToken ct);
        TokenResult? Authenticate(string email, string password, bool hashPassword = true);
        int? GetUserFromToken(string token);
        string? GetRoleFromToken(string token);
    }
}
