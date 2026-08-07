namespace Catalog.Auth.Services;

using System.Security.Cryptography;

internal static class RefreshTokenHasher
{
    // SHA-256 hex = 64 chars — fits current Token MaxLength(128)
    public static string Hash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash); // uppercase hex; be consistent
    }
}