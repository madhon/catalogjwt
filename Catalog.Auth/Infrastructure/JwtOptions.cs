namespace Catalog.Auth.Infrastructure;

using System.ComponentModel.DataAnnotations;

internal sealed class JwtOptions
{
    public const string Jwt = "jwt";

    [Required]
    public string Secret { get; set; } = null!;

    [Required]
    public string Audience { get; set; } = null!;

    [Required]
    public string Issuer { get; set; } = null!;

    /// <summary>Access token lifetime in minutes.</summary>
    [Range(1, 1440)]
    public int AccessTokenMinutes { get; set; } = 15;
    /// <summary>Refresh token lifetime in days.</summary>
    [Range(1, 90)]
    public int RefreshTokenDays { get; set; } = 3;
}