namespace Catalog.Auth.Services;

using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;

[RegisterScoped]
internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenService jwtTokenService;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly TimeProvider timeProvider;
    private readonly AuthDbContext dbContext;

    public AuthenticationService(IJwtTokenService jwtTokenService, UserManager<ApplicationUser> userManager, TimeProvider timeProvider, AuthDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(jwtTokenService);
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(timeProvider);
        ArgumentNullException.ThrowIfNull(dbContext);

        this.jwtTokenService = jwtTokenService;
        this.userManager = userManager;
        this.timeProvider = timeProvider;
        this.dbContext = dbContext;
    }

    public async Task<ErrorOr<IdentityResult>> CreateUser(string email, string password, string fullName, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

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

        ct.ThrowIfCancellationRequested();

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

            var result = jwtTokenService.CreateToken(additionalClaims, roles);
            var created = timeProvider.GetUtcNow().UtcDateTime;

            result.RefreshToken = CreateRefreshToken();
            result.RefreshExpiresAt = created.AddDays(3);

            // Save the refresh token to the database
            var refreshToken = new RefreshToken
            {
                Token = result.RefreshToken,
                UserId = user.Id,
                Created = created,
                Expires = result.RefreshExpiresAt,
            };
            dbContext.RefreshTokens.Add(refreshToken);
            await dbContext.SaveChangesAsync();

            return result;
        }

        return Errors.User.InvalidCredentials;
    }

    public async Task<TokenResult> Refresh(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        var additionalClaims = GenerateClaims(user!, user!.Email!);
        var roles = await userManager.GetRolesAsync(user);

        var result = jwtTokenService.CreateToken(additionalClaims, roles);
        var created = timeProvider.GetUtcNow().UtcDateTime;

        result.RefreshToken = CreateRefreshToken();
        result.RefreshExpiresAt = created.AddDays(3);

        // Save the refresh token to the database
        var refreshToken = new RefreshToken
        {
            Token = result.RefreshToken,
            UserId = user.Id,
            Created = created,
            Expires = result.RefreshExpiresAt,
        };
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync();

        return result;
    }

    private static Dictionary<string, object> GenerateClaims(ApplicationUser user, string email)
    {
        return new Dictionary<string, object>(capacity: 5, comparer: StringComparer.Ordinal)
        {
            { JwtRegisteredClaimNames.Sub, user.UserName! },
            { JwtClaimTypes.UserId, user.Id },
            { JwtRegisteredClaimNames.Email, email },
            { JwtClaimTypes.GrantType, "password" },
            { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N") },
        };
    }

    public string CreateRefreshToken()
    {
        // A refresh token is just a large cryptographically-random value.
        // RandomNumberGenerator replaces the obsolete RNGCryptoServiceProvider.
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}