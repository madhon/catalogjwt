namespace Catalog.Auth.Services;

using Microsoft.IdentityModel.JsonWebTokens;

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

        var result = await CreateUserAsync(user, password).ConfigureAwait(false);
        if (result.Succeeded)
        {
            result = await userManager.AddToRoleAsync(user, "read").ConfigureAwait(false);
            if (result.Succeeded)
            {
                return result;
            }
        }

        return Error.Failure("createuser.error", GenerateErrorString(result));
    }

    private async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
    {
        return await userManager.CreateAsync(user, password).ConfigureAwait(false);
    }

    private static string GenerateErrorString(IdentityResult result)
    {
        var errorString = new StringBuilder();
        foreach (var error in result.Errors)
        {
            errorString.AppendLine(error.Description);
        }
        return errorString.ToString();
    }

    public async Task<ErrorOr<TokenResult>> Authenticate(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
        if (user is not null && await userManager.CheckPasswordAsync(user, password).ConfigureAwait(false))
        {
            var additionalClaims = GenerateClaims(user, email);
            var roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);

            return jwtTokenService.CreateToken(additionalClaims, roles, 45);
        }

        return Errors.User.InvalidCredentials;
    }

    private static Dictionary<string, object> GenerateClaims(ApplicationUser user, string email)
    {
        return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            { JwtRegisteredClaimNames.Sub, user.UserName! },
            { JwtClaimTypes.UserId, user.Id },
            { JwtRegisteredClaimNames.Azp, email },
            { JwtClaimTypes.GrantType, "password" },
            { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
        };
    }
}