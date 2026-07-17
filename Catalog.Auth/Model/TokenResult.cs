namespace Catalog.Auth.Model;

internal sealed class TokenResult
{
    public required string Token { get; set; } = null!;

    public int ExpiresIn { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTime RefreshExpiresAt { get; set; }
}