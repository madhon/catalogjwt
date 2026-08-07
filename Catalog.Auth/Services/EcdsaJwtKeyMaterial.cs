namespace Catalog.Auth.Services;

using System.Security.Cryptography;

internal sealed class EcdsaJwtKeyMaterial : IDisposable
{
    private readonly ECDsa ecdsa;
    public SecurityKey SecurityKey { get; }
    public SigningCredentials? SigningCredentials { get; }

    public EcdsaJwtKeyMaterial(string pem, bool isPrivate)
    {
        ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(pem.Replace("\\n", "\n", StringComparison.Ordinal));
        SecurityKey = new ECDsaSecurityKey(ecdsa)
        {
            KeyId = Convert.ToHexString(SHA256.HashData(ecdsa.ExportSubjectPublicKeyInfo()))[..16],
        };

        SigningCredentials = isPrivate
            ? new SigningCredentials(SecurityKey, SecurityAlgorithms.EcdsaSha256)
            : null;

    }
    public void Dispose() => ecdsa.Dispose();
}