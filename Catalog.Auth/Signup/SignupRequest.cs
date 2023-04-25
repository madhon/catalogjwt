namespace Catalog.Auth.Signup
{
    public record SignupRequest
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string Fullname { get; init; } = null!;
    }
}
