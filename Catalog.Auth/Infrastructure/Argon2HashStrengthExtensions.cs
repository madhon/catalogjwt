namespace Catalog.Auth.Infrastructure;

using NSec.Cryptography;

internal static class Argon2HashStrengthExtensionsNSec
{
    // Parameter values mirror libsodium's OPSLIMIT/MEMLIMIT presets so that the
    // security profile is equivalent to the sodium-core originals.
    public static Argon2Parameters ToArgon2Parameters(this Argon2HashStrength strength) =>
        strength switch
        {
            Argon2HashStrength.Interactive => new Argon2Parameters
            {
                DegreeOfParallelism = 1,
                MemorySize = 65536,   // 64 MiB  (MEMLIMIT_INTERACTIVE)
                NumberOfPasses = 2,   // OPSLIMIT_INTERACTIVE
            },
            Argon2HashStrength.Moderate => new Argon2Parameters
            {
                DegreeOfParallelism = 1,
                MemorySize = 262144,  // 256 MiB (MEMLIMIT_MODERATE)
                NumberOfPasses = 3,   // OPSLIMIT_MODERATE
            },
            Argon2HashStrength.Medium => new Argon2Parameters
            {
                DegreeOfParallelism = 1,
                MemorySize = 262144,  // 256 MiB
                NumberOfPasses = 3,
            },
            Argon2HashStrength.Sensitive => new Argon2Parameters
            {
                DegreeOfParallelism = 1,
                MemorySize = 1048576, // 1 GiB   (MEMLIMIT_SENSITIVE)
                NumberOfPasses = 4,   // OPSLIMIT_SENSITIVE
            },
            _ => new Argon2Parameters
            {
                DegreeOfParallelism = 1,
                MemorySize = 262144,
                NumberOfPasses = 3,
            },
        };
}