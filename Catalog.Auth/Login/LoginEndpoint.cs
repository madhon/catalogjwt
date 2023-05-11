﻿namespace Catalog.Auth.Login
{
    public static class LoginEndpoint
    {
        public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
        {
            app.MapPost("api/v{version:apiVersion}/auth/login", 
                    async Task<Results<Ok<LoginResponse>, ValidationProblem, ProblemHttpResult>>
                    (LoginRequest request, IValidator < LoginRequest > loginValidator, IAuthenticationService authenticationService, CancellationToken ct) =>
                    {
                    var validationResult = await loginValidator.ValidateAsync(request, ct).ConfigureAwait(false);

                    if (!validationResult.IsValid)
                    {

                        return TypedResults.ValidationProblem(validationResult.ToDictionary());
                    }

                    var authenticationResult = await authenticationService.Authenticate(request.Email, request.Password).ConfigureAwait(false);

                    if (authenticationResult.IsError)
                    {
                        return TypedResults.Problem(
                            title: "Authentication error",
                            detail: authenticationResult.Errors.First().Description,
                            statusCode: StatusCodes.Status401Unauthorized
                        );
                    }

                    var response = new LoginResponse
                    {
                        AccessToken = authenticationResult.Value.Token,
                        ExpiresIn = authenticationResult.Value.ExpiresIn,
                    };

                    return TypedResults.Ok(response);
                    })
                .AllowAnonymous()
                .WithName("auth.login")
                .WithDescription("Login to API")
                .WithTags("Auth")
                .WithApiVersionSet(versionSet)
                .Accepts<LoginRequest>("application/json")
                .Produces<LoginResponse>(200, "application/json")
                .Produces<UnauthorizedResult>()
                .ProducesValidationProblem()
                .WithOpenApi();

            return app;
        }

    }
}
