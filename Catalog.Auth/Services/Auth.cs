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

        public async Task<string?> Authenticate(string username, string password, bool hashPassword = true)
        {
            var hashPwd = hashPassword == true ? password.Hash() : password;

            var user = await authContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == username.ToLower() && u.Password == hashPwd)
                .ConfigureAwait(false);

            if (user is null)
            {
                return null;
            }

            var perform_auth = Authenticate(user.Id, user.Role);
            return perform_auth;
        }

        public int? GetUserFromToken(string token)
        {

            var claims = auth.GetClaims(token.Replace("Bearer", string.Empty).Trim(), _configuration["jwt:issuer"], _configuration["jwt:audience"]);
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

            var claims = auth.GetClaims(token.Replace("Bearer", string.Empty).Trim(), _configuration["jwt:issuer"], _configuration["jwt:audience"]);
            if (claims is not null)
            {
                string[] clms = claims.Select(x => x.Value).ToArray();
                string role = clms[1];
                return role;
            }
            return null;
        }

        private string Authenticate(int userId, string role)
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
