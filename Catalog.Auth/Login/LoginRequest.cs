namespace Catalog.Auth.Login;

internal sealed record LoginRequest
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}