namespace Catalog.Auth.Services
{
    using System.Security.Claims;
    using Catalog.Auth.Extensions;
    using Catalog.Auth.Infrastructure;
    using Microsoft.EntityFrameworkCore;

    public class Auth : IAuth
    {
        private readonly IAuthenticate auth;
        private readonly IConfiguration _configuration;
        private readonly AuthContext authContext;


        public Auth(AuthContext authContext,  IAuthenticate auth, IConfiguration configuration)
        {
            this.authContext = authContext;
            this.auth = auth;
            _configuration = configuration;
            this.auth.SecretKey = _configuration["jwt:secret"];
        }

        public async Task<string?> AuthenticateAsync(string email, string password, bool hashPassword = true)
        {

#pragma warning disable MA0011 // IFormatProvider is missing
            var user = await authContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower())
                .ConfigureAwait(false);
#pragma warning restore MA0011 // IFormatProvider is missing

            if (user is null)
            {
                return null;
            }

            var verified = password.VerifyHash(user.Password);

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

            var claims = auth.GetClaims(parsedToken, _configuration["jwt:issuer"], _configuration["jwt:audience"]);
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
            var claims = auth.GetClaims(parsedToken, _configuration["jwt:issuer"], _configuration["jwt:audience"]);
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
            var result = auth.CreateToken(claims, _configuration["jwt:issuer"], _configuration["jwt:audience"], 45);
            return result;
        }
    }
}
