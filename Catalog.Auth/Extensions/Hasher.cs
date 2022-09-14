namespace Catalog.Auth.Extensions
{
    using BCrypt.Net;

    public static class Hasher
    {
        public static string Hash(this string data)
        {
            return BCrypt.HashPassword(data);
        }

        public static bool VerifyHash(this string data, string hash)
        {
            return BCrypt.Verify(data, hash);
        }
    }
}
