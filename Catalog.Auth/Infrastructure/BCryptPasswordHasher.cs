﻿namespace Catalog.Auth.Infrastructure
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;

    public class BCryptPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        private readonly BCryptPasswordHasherOptions options;

        public BCryptPasswordHasher(IOptions<BCryptPasswordHasherOptions> optionsAccessor = null)
        {
            options = optionsAccessor?.Value ?? new BCryptPasswordHasherOptions();
        }

        public string HashPassword(TUser user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }
            
            return BCrypt.Net.BCrypt.HashPassword(password, options.WorkFactor);
        }

        public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                throw new ArgumentNullException(nameof(hashedPassword));
            }

            if (string.IsNullOrWhiteSpace(providedPassword))
            {
                throw new ArgumentNullException(nameof(providedPassword));
            }

            var isValid = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);

            if (isValid && BCrypt.Net.BCrypt.PasswordNeedsRehash(hashedPassword, options.WorkFactor))
            {
                return PasswordVerificationResult.SuccessRehashNeeded;
            }

            return isValid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;

        }
    }
}