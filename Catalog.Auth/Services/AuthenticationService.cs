namespace Catalog.Auth.Services;

using Microsoft.IdentityModel.JsonWebTokens;

[RegisterScoped]
internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenService jwtTokenService;
    private readonly UserManager<ApplicationUser> userManager;

    public AuthenticationService(IJwtTokenService jwtTokenService, UserManager<ApplicationUser> userManager)
    {
        this.jwtTokenService = jwtTokenService;
        this.userManager = userManager;
    }

    public async Task<ErrorOr<IdentityResult>> CreateUser(string email, string password, string fullName, CancellationToken ct)
    {
        var user = new ApplicationUser
        {
            Email = email,
            UserName = email,
        };

        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            return Error.Failure("createuser.error", GenerateErrorString(createResult));
        }

        var roleResult = await userManager.AddToRoleAsync(user, "read");
        if (!roleResult.Succeeded)
        {
            // optional: keep things atomic
            await userManager.DeleteAsync(user);
            return Error.Failure("createuser.error", GenerateErrorString(roleResult));
        }

        return roleResult;
    }

    private static string GenerateErrorString(IdentityResult result)
    {
        var errorString = new StringBuilder(capacity: 128);
        foreach (var error in result.Errors)
        {
            errorString.AppendLine(error.Description);
        }
        return errorString.ToString();
    }

    public async Task<ErrorOr<TokenResult>> Authenticate(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
        if (user is not null && await userManager.CheckPasswordAsync(user, password))
        {
            var additionalClaims = GenerateClaims(user, email);
            var roles = await userManager.GetRolesAsync(user);

            return jwtTokenService.CreateToken(additionalClaims, roles);
        }

        return Errors.User.InvalidCredentials;
    }

    private static Dictionary<string, object> GenerateClaims(ApplicationUser user, string email)
    {
        return new Dictionary<string, object>(capacity: 5, comparer: StringComparer.Ordinal)
        {
            { JwtRegisteredClaimNames.Sub, user.UserName! },
            { JwtClaimTypes.UserId, user.Id },
            { JwtRegisteredClaimNames.Azp, email },
            { JwtClaimTypes.GrantType, "password" },
            { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N") },
        };
    }
}