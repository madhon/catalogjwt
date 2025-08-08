namespace Catalog.Auth.Signup;

internal sealed record SignupRequest(string Email, string Password, string Fullname);
