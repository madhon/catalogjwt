namespace Catalog.Auth.Signup
{
    public static class SignUpEndpoint
    {
        public static IEndpointRouteBuilder MapSignUpEndpoint(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
        {
            app.MapPost("api/v{version:apiVersion}/auth/signup",
                async 
                (SignupRequest request, IAuthenticationService authenticationService, CancellationToken ct) =>
                {

                var result = await authenticationService.CreateUser(request.Email, request.Password, request.Fullname, ct).ConfigureAwait(false);

                if (!result.IsError)
                {
                    return Results.Ok(new SignupResponse(true, "User created successfully"));
                }
                else
                {
                    return Results.BadRequest(new SignupResponse(false, $"Error creating user {result.Value}"));
                }
                })
                .AllowAnonymous()
                .WithName("auth.signup")
                .WithDescription("Sign Up to API")
                .WithTags("Auth")
                .WithApiVersionSet(versionSet)
                .Accepts<SignupRequest>("application/json")
                .Produces<SignupResponse>(200, "application/json")
                .Produces<BadRequest>()
                .ProducesValidationProblem()
                .WithOpenApi();

            return app;
        }

    }
}
