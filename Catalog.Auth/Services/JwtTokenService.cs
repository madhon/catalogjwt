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
            var signingKeyBase64 = jwtOptions.Value.Secret;
            var signingKeyBytes = Encoding.ASCII.GetBytes(signingKeyBase64);
            var signingKey = new SymmetricSecurityKey(signingKeyBytes);
            signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);
            issuer = jwtOptions.Value.Issuer;
            audience = jwtOptions.Value.Audience;
        }

        public TokenResult CreateToken(IDictionary<string, object> claims, int expiresInMinutes = 30)
        {
            var tokenHandler = new JsonWebTokenHandler();
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                SigningCredentials = signingCredentials,
                Issuer = issuer,
                Audience = audience,
                Claims = claims
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
