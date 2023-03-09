namespace Catalog.Auth.Services
{
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.JsonWebTokens;

    public class JwtTokenService : IJwtTokenService
    {
        private readonly SigningCredentials signingCredentials;
        private readonly string issuer;
        private readonly string audience;

        public JwtTokenService(IOptions<JwtOptions> jwtOptions)
        {
            ArgumentNullException.ThrowIfNull(jwtOptions);

            var signingKeyBase64 = jwtOptions.Value.Secret;
            var signingKeyBytes = Encoding.ASCII.GetBytes(signingKeyBase64);
            var signingKey = new SymmetricSecurityKey(signingKeyBytes);
            signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);
            issuer = jwtOptions.Value.Issuer;
            audience = jwtOptions.Value.Audience;
        }

        public TokenResult CreateToken(IDictionary<string, object> claims, IEnumerable<string> roles, int expiresInMinutes = 30)
        {
            var tokenHandler = new JsonWebTokenHandler();

            var issuedAt = DateTime.UtcNow;

            var claimsIdentity = new ClaimsIdentity();
            foreach (var role in roles)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                IssuedAt = issuedAt,
                NotBefore = issuedAt,
                Expires = issuedAt.AddMinutes(expiresInMinutes),
                SigningCredentials = signingCredentials,
                Claims = claims,
                Subject = claimsIdentity
            };
            
            var expiresIn = TimeSpan.FromMinutes(expiresInMinutes);
            
            var result = new TokenResult
            {
                Token = tokenHandler.CreateToken(tokenDescriptor),
                ExpiresIn =  Convert.ToInt32(expiresIn.TotalSeconds)
            };

            return result;
        }
    }
}
