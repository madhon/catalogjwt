namespace Catalog.Auth.Services
{
    public interface IAuthenticationService
    {
        Task CreateUser(string email, string password, string fullName, CancellationToken ct);
        string? Authenticate(string email, string password, bool hashPassword = true);
        int? GetUserFromToken(string token);
        string? GetRoleFromToken(string token);
    }
}
