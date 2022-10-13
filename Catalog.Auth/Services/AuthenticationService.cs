namespace Catalog.Auth.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IJwtTokenService jwtTokenService;
        private readonly IArgonService argonService;
        private readonly AuthContext authContext;
        
        private static readonly Func<AuthContext, string, User?> getUser =
            EF.CompileQuery((AuthContext db, string email) =>
                db.Users.AsNoTracking()
                    .FirstOrDefault(u => u.Email.ToLower() == email.ToLower()));
        
        public AuthenticationService(AuthContext authContext, IJwtTokenService jwtTokenService, IArgonService argonService)
        {
            this.authContext = authContext;
            this.jwtTokenService = jwtTokenService;
            this.argonService = argonService;
        }

        public async Task CreateUser(string email, string password, string fullName, CancellationToken ct)
        {
            var saltHash = argonService.CreateHashAndSalt(password);

            authContext.Users.Add(new User
            {
                Id = Ulid.NewUlid(),
                Email = email,
                Fullname = fullName,
                Salt = saltHash.Salt,
                Password = saltHash.Hash,
            });

            await authContext.SaveChangesAsync(ct).ConfigureAwait(false);
        }


        public string? Authenticate(string email, string password, bool hashPassword = true)
        {

            var user = getUser(authContext, email);

            if (user is null)
            {
                return null;
            }

            var salt = user.Salt;
            var hash = user.Password;

            var verified = argonService.VerifyHash(password, salt, hash);
            if (!verified)
            {
                return null;
            }

            var perform_auth = Authenticate(user.Id, user.Role);
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

        private string Authenticate(Ulid userId, string role)
        {
            var claims = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.Name, userId.ToString()),
                    new Claim(ClaimTypes.Role, role),
            });
            var result = jwtTokenService.CreateToken(claims, 45);
            return result;
        }
    }
}
