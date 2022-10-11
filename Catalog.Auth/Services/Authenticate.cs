﻿namespace Catalog.Auth.Services
{
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.JsonWebTokens;

    public class Authenticate : IAuthenticate
    {
        private readonly JwtOptions jwtOptions;

        public Authenticate(IOptions<JwtOptions> jwtOptions)
        {
            this.jwtOptions = jwtOptions.Value;
        }

        public IEnumerable<Claim>? GetClaims(string token)
        {
            var tokenHandler = new JsonWebTokenHandler();
            var validate = ValidateToken(token);
            return validate.IsValid ? validate.Claims : null;
        }

        private (bool IsValid, IEnumerable<Claim> Claims) ValidateToken(string token)
        {
            try
            {
                var key = Encoding.ASCII.GetBytes(jwtOptions.Secret);
                var tokenHandler = new JsonWebTokenHandler();
                var validationResult =  tokenHandler.ValidateToken(token, new TokenValidationParameters
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

        public string CreateToken(ClaimsIdentity claimsIdentity, int expiresInMinutes = 30)
        {
            var tokenHandler = new JsonWebTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtOptions.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return token;
        }
    }
}
