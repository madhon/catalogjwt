namespace Catalog.Auth.Infrastructure;

using ErrorOr;

internal static partial class Errors
{
    internal static class User
    {
        public static readonly Error InvalidCredentials = Error.Failure("User.InvalidCreds", "Invalid Credentials");
    }

}