namespace Catalog.Auth.Model
{
    public class User
    {
        public Ulid Id { get; set; }
        public string Role { get; set; } = "USER";
        public string Email { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
    }
}
