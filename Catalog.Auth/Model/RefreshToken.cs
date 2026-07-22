namespace Catalog.Auth.Model;

internal sealed class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
    public DateTime? Revoked { get; set; }

    // When this token is rotated, we record the token that replaced it.
    public string? ReplacedByToken { get; set; }

    // A token is only usable if it has not been revoked and has not expired.
    public bool IsActive => Revoked is null && DateTime.UtcNow < Expires;
}
