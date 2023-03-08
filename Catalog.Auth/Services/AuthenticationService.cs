namespace Catalog.Auth.Services
{
    using Microsoft.IdentityModel.JsonWebTokens;

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IJwtTokenService jwtTokenService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AuthenticationService(AuthContext authContext, 
            IJwtTokenService jwtTokenService, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager)
        {
            this.jwtTokenService = jwtTokenService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<ErrorOr<IdentityResult>> CreateUser(string email, string password, string fullName, CancellationToken ct)
        {
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
            };

            var result = await userManager.CreateAsync(user, password).ConfigureAwait(false);

            return result;
        }

        public async Task<ErrorOr<TokenResult>> Authenticate(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user is not null && await userManager.CheckPasswordAsync(user, password).ConfigureAwait(false))
            {
                // allowed to login
                var userRoles = await userManager.GetRolesAsync(user);

                var additionalClaims = new Dictionary<string, object>
                {
                    { JwtRegisteredClaimNames.Sub, user.UserName},
                    { JwtClaimTypes.UserId, user.Id},
                    { JwtRegisteredClaimNames.Azp, email },
                    { JwtClaimTypes.GrantType, "password" },
                    { ClaimTypes.Role, "read"},
                    { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() }
                };

                foreach (var userRole in userRoles)
                {
                    additionalClaims.Add(ClaimTypes.Role, userRole);
                }

                return jwtTokenService.CreateToken(additionalClaims, 45);
            }

            return Errors.User.InvalidCredentials;
           
        }
    }
}
