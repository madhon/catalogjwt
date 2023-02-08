namespace Catalog.Auth.Services
{
    using Microsoft.AspNetCore.Authorization;

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

            var additionalClaims = new Dictionary<string, object>
            {
                { JwtClaimTypes.AuthorizedParty, email },
                { JwtClaimTypes.GrantType, "password" },
                { JwtClaimTypes.JwtId, Guid.NewGuid().ToString() }
            };

            var perform_auth = GetToken(user.Id, "Role", additionalClaims);
            return perform_auth;
        }

        public int? GetUserFromToken(string token)
        {
            var parsedToken = token.Replace("Bearer", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();

            var claims = jwtTokenService.GetClaims(parsedToken);
            if (claims is not null)
            {
                string[] clms = claims.Select(x => x.Value).ToArray();
                var userId = clms[0];
                return int.Parse(userId);
            }
            return null;
        }

        public string? GetRoleFromToken(string token)
        {
            var parsedToken = token.Replace("Bearer", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
            var claims = jwtTokenService.GetClaims(parsedToken);
            if (claims is not null)
            {
                string[] clms = claims.Select(x => x.Value).ToArray();
                string role = clms[1];
                return role;
            }
            return null;
        }

        private TokenResult GetToken(string userId, string role, IDictionary<string, object> additionalClaims)
        {
            var claims = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.Name, userId.ToString()),
                    new Claim(ClaimTypes.Role, role),
            });
            var result = jwtTokenService.CreateToken(claims, additionalClaims, 45);
            return result;
        }
    }
}
