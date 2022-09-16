namespace Catalog.Auth.Services
{
    public interface IArgonService
    {
        byte[] CreateSalt();
        bool VerifyHash(string password, string salt, string hash);
        bool VerifyHash(string password, byte[] salt, byte[] hash);
        string HashPassword(string password, byte[] salt);
        public (string Hash, string Salt) CreateHashAndSalt(string password);
    }
}
