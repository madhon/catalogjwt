namespace Catalog.API.Infrastructure.Authentication.Settings
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    public class AuthenticationSettings
    {

        private readonly string secretBase64 = default!;

        [Required, MinLength(10)]
        public string Secret
        {
            get => secretBase64;
            init { secretBase64 = value;
                JwtSigningKey = Encoding.ASCII.GetBytes(value);
            }
        }

        [Required, MinLength(1)]
        public string Audience { get; init; } = default!;

        [Required, MinLength(1)]
        public string Issuer { get; init; } = default!;

        public byte[] JwtSigningKey { get; private set; } = default!;
    }
}
