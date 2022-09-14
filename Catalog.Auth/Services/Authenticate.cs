﻿namespace Catalog.Auth.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;

    public class Authenticate : IAuthenticate
    {
        public string SecretKey { get; set; } = string.Empty;

        public IEnumerable<Claim>? GetClaims(string token, string issuer, string audience)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secToken = tokenHandler.ReadJwtToken(token.Replace("Bearer", string.Empty, StringComparison.OrdinalIgnoreCase).Trim());
            bool validate = ValidateToken(token, issuer, audience);
            return validate ? secToken.Claims : null;
        }

        bool ValidateToken(string token, string issuer, string audience)
        {
            try
            {
                var key = Encoding.ASCII.GetBytes(SecretKey);
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken securityToken);

                return securityToken != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string CreateToken(ClaimsIdentity claimsIdentity, string issuer, string audience, int expiresInMinutes = 30)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string result = tokenHandler.WriteToken(token);
            return result;
        }
    }
}
