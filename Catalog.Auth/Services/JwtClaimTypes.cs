namespace Catalog.Auth.Services
{
    public static class JwtClaimTypes
    {
        /// <summary>
        /// The type of grant requested, eg client credentials or password
        /// </summary>
        public const string GrantType = "gty";

        public const string Role = "role";

        public const string UserId = "userId";
    }
}
