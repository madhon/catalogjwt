namespace Catalog.Auth.Services
{
    using Microsoft.Extensions.Options;
    using System.IdentityModel.Tokens.Jwt;

    public class Authenticate : IAuthenticate
    {
        private readonly JwtOptions jwtOptions;

        public Authenticate(IOptions<JwtOptions> jwtOptions)
        {
            this.jwtOptions = jwtOptions.Value;
        }

        public IEnumerable<Claim>? GetClaims(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secToken = tokenHandler.ReadJwtToken(token.Replace("Bearer", string.Empty, StringComparison.OrdinalIgnoreCase).Trim());
            bool validate = ValidateToken(token);
            return validate ? secToken.Claims : null;
        }

        private bool ValidateToken(string token)
        {
            try
            {
                var key = Encoding.ASCII.GetBytes(jwtOptions.Secret);
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken securityToken);

                return securityToken != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string CreateToken(ClaimsIdentity claimsIdentity, int expiresInMinutes = 30)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtOptions.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string result = tokenHandler.WriteToken(token);
            return result;
        }
    }
}
