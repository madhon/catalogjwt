namespace Catalog.API.Infrastructure.Authentication.Settings;

using System.ComponentModel.DataAnnotations;

public class AuthenticationSettings
{
    public const string SectionName = "AuthenticationSettings";

    [Required, MinLength(10)]
    public string Secret{ get; set; } = null!;

    [Required, MinLength(1)]
    public string Audience { get; set; } = null!;

    [Required, MinLength(1)]
    public string Issuer { get; set; } = null!;
}