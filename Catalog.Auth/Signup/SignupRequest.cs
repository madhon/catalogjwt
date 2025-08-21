namespace Catalog.Auth.Signup;

public sealed record SignupRequest(string Email, string Password, string Fullname);
