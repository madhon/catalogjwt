namespace Catalog.Auth.Infrastructure
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;
    using Sodium;
    
    public class Argon2PasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
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

            return PasswordHash.ArgonHashString(password, ParseStrength()).TrimEnd('\0');
        }

        public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            ArgumentNullException.ThrowIfNull(hashedPassword);
            ArgumentNullException.ThrowIfNull(providedPassword);

            var isValid = PasswordHash.ArgonHashStringVerify(hashedPassword, providedPassword);

            if (isValid && PasswordHash.ArgonPasswordNeedsRehash(hashedPassword, ParseStrength()))
            {
                return PasswordVerificationResult.SuccessRehashNeeded;
            }

            return isValid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        private PasswordHash.StrengthArgon ParseStrength()
        {
            switch (options.Strength)
            {
                case Argon2HashStrength.Interactive:
                    return PasswordHash.StrengthArgon.Interactive;
                case Argon2HashStrength.Moderate:
                    return PasswordHash.StrengthArgon.Moderate;
                case Argon2HashStrength.Sensitive:
                    return PasswordHash.StrengthArgon.Sensitive;
                case Argon2HashStrength.Medium:
                    return PasswordHash.StrengthArgon.Medium;
                default:
                    return PasswordHash.StrengthArgon.Medium;
            }
        }
    }
}
