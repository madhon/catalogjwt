namespace Catalog.Gateway.Extensions;

using System.Security.Cryptography;

internal sealed class EcdsaJwtKeyMaterial : IDisposable
{
    private readonly ECDsa ecdsa;
    public SecurityKey SecurityKey { get; }
    public SigningCredentials? SigningCredentials { get; }
    private const string NistP256Oid = "1.2.840.10045.3.1.7";

    public EcdsaJwtKeyMaterial(string pem, bool isPrivate)
    {
        ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(pem.Replace("\\n", "\n", StringComparison.Ordinal));

        var parameters = ecdsa.ExportParameters(includePrivateParameters: false);
        if (!string.Equals(
                parameters.Curve.Oid.Value,
                NistP256Oid,
                StringComparison.Ordinal))
        {
            ecdsa.Dispose();
            throw new CryptographicException(
                "ES256 JWT keys must use the NIST P-256 curve.");
        }

        var key = new ECDsaSecurityKey(ecdsa)
        {
            KeyId = Convert.ToHexString(SHA256.HashData(ecdsa.ExportSubjectPublicKeyInfo()))[..16],
            CryptoProviderFactory = new CryptoProviderFactory
            {
                CacheSignatureProviders = true,
            },
        };

        SecurityKey = key;

        SigningCredentials = isPrivate
            ? new SigningCredentials(SecurityKey, SecurityAlgorithms.EcdsaSha256)
            {
                CryptoProviderFactory = key.CryptoProviderFactory,
            }
            : null;

    }
    public void Dispose() => ecdsa.Dispose();
}