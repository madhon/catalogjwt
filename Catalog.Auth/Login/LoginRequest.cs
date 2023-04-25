namespace Catalog.Auth.Login
{
    public record LoginRequest
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
