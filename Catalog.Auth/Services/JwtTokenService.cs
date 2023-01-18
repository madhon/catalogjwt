namespace Catalog.Auth.Services
{
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.JsonWebTokens;

    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions jwtOptions;

        public JwtTokenService(IOptions<JwtOptions> jwtOptions)
        {
            this.jwtOptions = jwtOptions.Value;
        }

        public IEnumerable<Claim>? GetClaims(string token)
        {
            var validate = ValidateToken(token);
            return validate.IsValid ? validate.Claims : null;
        }

        public TokenResult CreateToken(ClaimsIdentity claimsIdentity, IDictionary<string, object> additionalClaims, int expiresInMinutes = 30)
        {
            var tokenHandler = new JsonWebTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtOptions.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                Claims = additionalClaims
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var expiresIn = TimeSpan.FromMinutes(expiresInMinutes);
            
            var result = new TokenResult
            {
                Token = token,
                ExpiresIn =  Convert.ToInt32(expiresIn.TotalSeconds)
            };

            return result;
        }

        private (bool IsValid, IEnumerable<Claim> Claims) ValidateToken(string token)
        {
            try
            {
                var key = Encoding.ASCII.GetBytes(jwtOptions.Secret);
                var tokenHandler = new JsonWebTokenHandler();
                var validationResult = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                });

                return (validationResult.IsValid, validationResult.ClaimsIdentity.Claims);
            }
            catch (Exception)
            {
                return (false, Enumerable.Empty<Claim>());
            }
        }
    }
}
