namespace Catalog.Auth.Infrastructure
{
    using ErrorOr;

    public static partial class Errors
    {
        public static class User
        {
            public static readonly Error InvalidCredentials = Error.Failure("User.InvalidCreds", "Invalid Credentials");
        }

    }
}
