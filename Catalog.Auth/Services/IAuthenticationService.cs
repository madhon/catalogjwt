namespace Catalog.Auth.Services;

using ErrorOr;

public interface IAuthenticationService
{
    Task<ErrorOr<IdentityResult>> CreateUser(string email, string password, string fullName, CancellationToken ct);
    Task<ErrorOr<TokenResult>> Authenticate(string email, string password);
}