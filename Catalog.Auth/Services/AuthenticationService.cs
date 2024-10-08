﻿namespace Catalog.Auth.Services;

using Microsoft.IdentityModel.JsonWebTokens;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenService jwtTokenService;
    private readonly UserManager<ApplicationUser> userManager;

    public AuthenticationService(AuthDbContext authContext, 
        IJwtTokenService jwtTokenService, 
        UserManager<ApplicationUser> userManager)
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

        IdentityResult result;
        result = await userManager.CreateAsync(user, password).ConfigureAwait(false);
            
        if (result.Succeeded)
        {
            result = await userManager.AddToRoleAsync(user, "read").ConfigureAwait(false);
            if (result.Succeeded)
            {
                return result;
            }
        }

        var errorString = new StringBuilder();
        foreach (var r in result.Errors)
        {
            errorString.AppendLine(r.Description);
        }

        return Error.Failure("createuser.error", errorString.ToString());
    }

    public async Task<ErrorOr<TokenResult>> Authenticate(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
        if (user is not null && await userManager.CheckPasswordAsync(user, password).ConfigureAwait(false))
        {
            // allowed to log in
            var additionalClaims = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { JwtRegisteredClaimNames.Sub, user!.UserName!},
                { JwtClaimTypes.UserId, user.Id},
                { JwtRegisteredClaimNames.Azp, email },
                { JwtClaimTypes.GrantType, "password" },
                { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
            };

            var roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
                
            return jwtTokenService.CreateToken(additionalClaims, roles, 45);
        }

        return Errors.User.InvalidCredentials;
    }
}