namespace Catalog.Auth.Infrastructure
{
    using System.ComponentModel.DataAnnotations;

    public class JwtOptions
    {
        public const string Jwt = "jwt";
        
        [Required]
        public string Secret { get; init; } = default!;
        
        [Required]
        public string Audience { get; init; } = default!;
        
        [Required]
        public string Issuer { get; init; } = default!;
        
    }
}
