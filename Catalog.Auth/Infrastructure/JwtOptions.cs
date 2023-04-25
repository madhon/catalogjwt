namespace Catalog.Auth.Infrastructure
{
    public class JwtOptions
    {
        public const string Jwt = "jwt";
        public string Secret { get; init; } = default!;
        public string Audience { get; init; } = default!;
        public string Issuer { get; init; } = default!;
    }
}
