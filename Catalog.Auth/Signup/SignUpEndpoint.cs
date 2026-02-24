using Microsoft.AspNetCore.Mvc;

namespace Catalog.Auth.Signup;

internal sealed class SignUpEndpointLoggerCategory;

internal static partial class SignUpEndpoint
{
    public static IEndpointRouteBuilder MapSignUpEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("auth/signup", HandleSignup)
            .AllowAnonymous()
            .WithName(nameof(HandleSignup))
            .WithDescription("Sign Up to API")
            .WithTags("Auth")
            .Accepts<SignupRequest>("application/json")
            .Produces<SignupResponse>(200, "application/json")
            .ProducesValidationProblem()
            .RequireRateLimiting(RateLimiterPolicies.RlPoicy);

        return app;
    }

    private static async Task<Results<Ok<SignupResponse>, ProblemHttpResult>> HandleSignup(SignupRequest request,
        IAuthenticationService authenticationService,
        [FromServices] ILogger<SignUpEndpointLoggerCategory> logger,
        CancellationToken ct)
    {
        LogUserSignup(logger, request.Email);

        var result = await authenticationService.CreateUser(request.Email, request.Password, request.Fullname, ct);

        return !result.IsError ?
            TypedResults.Ok(new SignupResponse(Success: true, "User created successfully")) :
            TypedResults.Problem(detail: result.Errors[0].Description,
                statusCode: 400,
                title: "Error Creating User");
    }

    [LoggerMessage(0, LogLevel.Information, "User Signed Up {userName}")]
    private static partial void LogUserSignup(ILogger<SignUpEndpointLoggerCategory> logger, string userName);
}