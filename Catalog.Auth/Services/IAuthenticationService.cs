namespace Catalog.Auth.Services
{
    public interface IAuthenticationService
    {
        Task<Result> CreateUser(string email, string password, string fullName, CancellationToken ct);
        Task<TokenResult?> Authenticate(string email, string password, bool hashPassword = true);
    }
}
