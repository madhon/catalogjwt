namespace Catalog.Auth.Services
{
    using Konscious.Security.Cryptography;
    using Microsoft.Extensions.Options;

    public class ArgonService : IArgonService
    {
        private readonly ArgonOptions argonOptions;

        public ArgonService(IOptions<ArgonOptions> argonOptions)
        {
            this.argonOptions = argonOptions.Value;
        }

        public byte[] CreateSalt()
        {
            var buffer = new byte[16];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buffer);
            return buffer;
        }

        public bool VerifyHash(string password, string salt, string hash)
        {
            var saltBytes = Convert.FromBase64String(salt);
            
            var hashBytes = Convert.FromBase64String(hash);

            var newHash = HashPassword(password, saltBytes);
            var newHashBytes = Convert.FromBase64String(newHash);
            return hashBytes.SequenceEqual(newHashBytes);
        }

        public bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = HashPassword(password, salt);
            var newHashBytes = Convert.FromBase64String(newHash);
            return hash.SequenceEqual(newHashBytes);
        }

        public string HashPassword(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = argonOptions.DegreeOfParallelism; 
            argon2.Iterations = argonOptions.Iterations;
            argon2.MemorySize = 1024 * argonOptions.MemorySize;

            var hashBytes = argon2.GetBytes(16);

            return Convert.ToBase64String(hashBytes);
        }

        public (string Hash, string Salt) CreateHashAndSalt(string password)
        {
            var salt = CreateSalt();
            var passwordHash = HashPassword(password, salt);
            var saltString = Convert.ToBase64String(salt);
            return (passwordHash, saltString);
        }
    }

}
