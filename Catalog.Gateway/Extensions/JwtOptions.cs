namespace Catalog.Gateway.Extensions;

internal sealed class JwtOptions
{
    public const string Jwt = "jwt";
    public string PublicKeyPem { get; set; } = null!;
    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
}