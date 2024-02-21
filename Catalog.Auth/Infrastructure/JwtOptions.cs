namespace Catalog.Auth.Infrastructure
{
    using System.ComponentModel.DataAnnotations;

    public class JwtOptions
    {
        public const string Jwt = "jwt";
        
        [Required]
        public string Secret { get; set; } = null!;
        
        [Required]
        public string Audience { get; set; } = null!;
        
        [Required]
        public string Issuer { get; set; } = null!;
        
    }
}
