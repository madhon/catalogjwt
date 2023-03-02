namespace Catalog.Auth.ViewModel
{
    public record SignUpModel
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string Fullname { get; init; } = null!;
    }
}
