namespace Catalog.Auth.Model;

public class TokenResult
{
    public required  string Token { get; set; } = null!;
    public int ExpiresIn { get; set; }
}