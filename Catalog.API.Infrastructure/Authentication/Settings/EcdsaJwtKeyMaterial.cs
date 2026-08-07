namespace Catalog.API.Infrastructure.Authentication.Settings;

using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

public sealed class EcdsaJwtKeyMaterial : IDisposable
{
    private readonly ECDsa ecdsa;
    public SecurityKey SecurityKey { get; }
    public SigningCredentials? SigningCredentials { get; }

    public EcdsaJwtKeyMaterial(string pem, bool isPrivate)
    {
        ArgumentNullException.ThrowIfNull(pem);

        ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(pem.Replace("\\n", "\n", StringComparison.Ordinal));
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