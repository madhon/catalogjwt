namespace Catalog.Auth.Services
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.IdentityModel.JsonWebTokens;

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IJwtTokenService jwtTokenService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IAuthorizationService authorizationService;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory;

        public AuthenticationService(AuthContext authContext, 
            IJwtTokenService jwtTokenService, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            IAuthorizationService authorizationService,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory)
        {
            this.jwtTokenService = jwtTokenService;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.authorizationService = authorizationService;
            this.userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        }

        public async Task<Result> CreateUser(string email, string password, string fullName, CancellationToken ct)
        {
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
            };

            var result = await userManager.CreateAsync(user, password).ConfigureAwait(false);

            return result.ToApplicationResult();
        }

        public async Task<TokenResult?> Authenticate(string email, string password, bool hashPassword = true)
        {
            var result = await signInManager.PasswordSignInAsync(email, password, false, false).ConfigureAwait(false);
            
            if (!result.Succeeded)
            {
                return null;
            }

            var user = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user is null)
            {
                return null;
            }

            var x = ClaimTypes.Role;

            var additionalClaims = new Dictionary<string, object>
            {
                { JwtRegisteredClaimNames.Sub, user.UserName},
                { JwtClaimTypes.UserId, user.Id},
                { JwtRegisteredClaimNames.Azp, email },
                { JwtClaimTypes.GrantType, "password" },
                { ClaimTypes.Role, "read"},
                { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() }
            };

            return jwtTokenService.CreateToken(additionalClaims, 45);

        }
    }
}
