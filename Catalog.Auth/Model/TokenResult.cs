namespace Catalog.Auth.Model
{
    public class TokenResult
    {
        public string Token { get; set; } = null!;
        public int ExpiresIn { get; set; }
    }
}
