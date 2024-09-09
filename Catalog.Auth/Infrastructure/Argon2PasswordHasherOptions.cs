namespace Catalog.Auth.Infrastructure;

public class Argon2PasswordHasherOptions
{
    /// <summary>
    /// Hash strength using pre-defined strengths from libsodium
    /// </summary>
    public Argon2HashStrength Strength { get; set; } = Argon2HashStrength.Interactive;
}