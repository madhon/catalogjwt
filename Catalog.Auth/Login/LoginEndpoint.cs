namespace Catalog.Auth.Login;

using Microsoft.AspNetCore.Mvc;

internal sealed class LoginEndpointLoggerCategory;

internal static partial class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", HandleLogin)
            .AllowAnonymous()
            .WithName(nameof(HandleLogin))
            .WithDescription("Login to API")
            .WithTags("Auth")
            .Accepts<LoginRequest>("application/json")
            .Produces<LoginResponse>(200, "application/json")
            .Produces<UnauthorizedHttpResult>()
            .Produces<ProblemHttpResult>()
            .ProducesValidationProblem();

        return app;
    }

    private static async Task<Results<Ok<LoginResponse>, ValidationProblem, ProblemHttpResult, UnauthorizedHttpResult>> HandleLogin(LoginRequest request,
        [FromServices] IValidator<LoginRequest> loginValidator,
        [FromServices] IAuthenticationService authenticationService,
        [FromServices] ILogger<LoginEndpointLoggerCategory> logger,
        [FromServices] ApiMetrics metrics,
        CancellationToken ct)
    {
        LogAuthenticating(logger, request.Email);

        var validationResult = await loginValidator.ValidateAsync(request, ct);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var authenticationResult = await authenticationService.Authenticate(request.Email, request.Password).ConfigureAwait(false);

        if (authenticationResult.IsError)
        {
            metrics.UserFailedLogin();
            return TypedResults.Unauthorized();
        }

        var response = new LoginResponse
        {
            AccessToken = authenticationResult.Value.Token,
            ExpiresIn = authenticationResult.Value.ExpiresIn,
        };

        metrics.UserLoggedIn();

        return TypedResults.Ok(response);
    }

    [LoggerMessage(0, LogLevel.Information, "Authenticating {userName}")]
    private static partial void LogAuthenticating(ILogger<LoginEndpointLoggerCategory> logger, string userName);
}