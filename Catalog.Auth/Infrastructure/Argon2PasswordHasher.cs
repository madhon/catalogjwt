namespace Catalog.Auth.Infrastructure;

using System.Collections.Frozen;
using System.Security.Cryptography;
using NSec.Cryptography;

internal sealed class Argon2PasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class

{
  // PHC string format prefix so we can identify our own hashes
    private const string HashPrefix = "$argon2id$v=19$";
    private const int SaltSize = 16;  // 16 bytes = libsodium's required salt length for Argon2id
    private const int HashSize = 32;  // 32 bytes output, matches sodium-core default

    private readonly Argon2PasswordHasherOptions _options;

    public Argon2PasswordHasher(IOptions<Argon2PasswordHasherOptions>? optionsAccessor = null)
    {
        _options = optionsAccessor?.Value ?? new Argon2PasswordHasherOptions();
    }

    public string HashPassword(TUser user, string password)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var argon2 = GetArgon2id(_options.Strength);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var hash = argon2.DeriveBytes(passwordBytes, salt, HashSize);

        // Encode as a simple portable format: "prefix|b64salt|b64hash"
        // This replicates the self-describing hash string sodium-core produced.
        var encoded = $"{HashPrefix}{_options.Strength}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        return encoded;
    }

    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
        ArgumentNullException.ThrowIfNull(hashedPassword);
        ArgumentNullException.ThrowIfNull(providedPassword);

        if (!TryParseHash(hashedPassword, out var storedStrength, out var salt, out var storedHash))
        {
            return PasswordVerificationResult.Failed;
        }

        var argon2 = GetArgon2id(storedStrength);
        var passwordBytes = Encoding.UTF8.GetBytes(providedPassword);
        var candidateHash = argon2.DeriveBytes(passwordBytes, salt, HashSize);

        if (!CryptographicOperations.FixedTimeEquals(candidateHash, storedHash))
        {
            return PasswordVerificationResult.Failed;
        }

        // Rehash needed if the stored strength differs from the current configured strength
        if (storedStrength != _options.Strength)
        {
            return PasswordVerificationResult.SuccessRehashNeeded;
        }

        return PasswordVerificationResult.Success;
    }

    private static bool TryParseHash(
        string hashedPassword,
        out Argon2HashStrength strength,
        out byte[] salt,
        out byte[] hash)
    {
        strength = default;
        salt = [];
        hash = [];

        // Expected format: "$argon2id$v=19$<Strength>$<b64salt>$<b64hash>"
        if (!hashedPassword.StartsWith(HashPrefix, StringComparison.Ordinal))
        {
            return false;
        }

        var remainder = hashedPassword[HashPrefix.Length..];
        var parts = remainder.Split('$');

        if (parts.Length != 3)
        {
            return false;
        }

        if (!Enum.TryParse<Argon2HashStrength>(parts[0], out strength))
        {
            return false;
        }

        try
        {
            salt = Convert.FromBase64String(parts[1]);
            hash = Convert.FromBase64String(parts[2]);
        }
        catch (FormatException)
        {
            return false;
        }

        return salt.Length == SaltSize && hash.Length == HashSize;
    }

    // private static Argon2id GetArgon2id(Argon2HashStrength strength)
    // {
    //     var parameters = strength.ToArgon2Parameters();
    //     return PasswordBasedKeyDerivationAlgorithm.Argon2id(parameters);
    // }

    private static readonly FrozenDictionary<Argon2HashStrength, Argon2id> Argon2Algorithms =
        Enum.GetValues<Argon2HashStrength>()
            .ToFrozenDictionary(
                strength => strength,
                strength => PasswordBasedKeyDerivationAlgorithm.Argon2id(strength.ToArgon2Parameters()));

    private static Argon2id GetArgon2id(Argon2HashStrength strength) =>
        Argon2Algorithms[strength];
}
