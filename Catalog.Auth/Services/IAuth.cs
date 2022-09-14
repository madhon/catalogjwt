namespace Catalog.Auth.Services
{
    public interface IAuth
    {
        Task<string?> AuthenticateAsync(string email, string password, bool hashPassword = true);
        int? GetUserFromToken(string token);
        string? GetRoleFromToken(string token);
    }
}
