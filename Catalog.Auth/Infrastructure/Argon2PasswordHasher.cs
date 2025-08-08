namespace Catalog.Auth.Infrastructure;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Sodium;

internal sealed class Argon2PasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
{
    private readonly Argon2PasswordHasherOptions options;

    /// <summary>
    /// Creates a new Argon2PasswordHasher.
    /// </summary>
    /// <param name="optionsAccessor">optional Argon2PasswordHasherOptions</param>
    public Argon2PasswordHasher(IOptions<Argon2PasswordHasherOptions>? optionsAccessor = null)
    {
        options = optionsAccessor?.Value ?? new Argon2PasswordHasherOptions();
    }

    public string HashPassword(TUser user, string password)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(password);

        return PasswordHash.ArgonHashString(password, options.Strength.ToStrengthArgon()).TrimEnd('\0');
    }

    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
        ArgumentNullException.ThrowIfNull(hashedPassword);
        ArgumentNullException.ThrowIfNull(providedPassword);

        var isValid = PasswordHash.ArgonHashStringVerify(hashedPassword, providedPassword);

        if (isValid && PasswordHash.ArgonPasswordNeedsRehash(hashedPassword, options.Strength.ToStrengthArgon()))
        {
            return PasswordVerificationResult.SuccessRehashNeeded;
        }

        return isValid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
    }
}

// Extension method to convert Argon2HashStrength to PasswordHash.StrengthArgon
#pragma warning disable MA0048
internal static class Argon2HashStrengthExtensions
#pragma warning restore MA0048
{
    public static PasswordHash.StrengthArgon ToStrengthArgon(this Argon2HashStrength strength)
    {
        return strength switch
        {
            Argon2HashStrength.Interactive => PasswordHash.StrengthArgon.Interactive,
            Argon2HashStrength.Moderate => PasswordHash.StrengthArgon.Moderate,
            Argon2HashStrength.Sensitive => PasswordHash.StrengthArgon.Sensitive,
            Argon2HashStrength.Medium => PasswordHash.StrengthArgon.Medium,
            _ => PasswordHash.StrengthArgon.Medium,
        };
    }
}