namespace Catalog.Auth.Login
{
    public static partial class LoginEndpoint
    {
        public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
        {
            app.MapPost("api/v{version:apiVersion}/auth/login", HandleLogin)
                .AllowAnonymous()
                .WithName("auth.login")
                .WithDescription("Login to API")
                .WithTags("Auth")
                .WithApiVersionSet(versionSet)
                .Accepts<LoginRequest>("application/json")
                .Produces<LoginResponse>(200, "application/json")
                .Produces<UnauthorizedHttpResult>()
                .Produces<ProblemHttpResult>()
                .ProducesValidationProblem()
                .WithOpenApi();

            return app;
        }

        private static async Task<Results<Ok<LoginResponse>, ValidationProblem, ProblemHttpResult, UnauthorizedHttpResult>> HandleLogin(LoginRequest request,
                        IValidator<LoginRequest> loginValidator,
                        IAuthenticationService authenticationService,
                        ILoggerFactory loggerFactory,
                        CancellationToken ct)
        {
            var logger = loggerFactory.CreateLogger("LoginEndpoint");

            LogAuthenticating(logger, request.Email);

            var validationResult = await loginValidator.ValidateAsync(request, ct).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }

            var authenticationResult = await authenticationService.Authenticate(request.Email, request.Password).ConfigureAwait(false);

            if (authenticationResult.IsError)
            {
                return TypedResults.Unauthorized();
            }

            var response = new LoginResponse
            {
                AccessToken = authenticationResult.Value.Token,
                ExpiresIn = authenticationResult.Value.ExpiresIn,
            };

            return TypedResults.Ok(response);
        }


        [LoggerMessage(0, LogLevel.Information, "Authenticating {userName}")]
        public static partial void LogAuthenticating(Microsoft.Extensions.Logging.ILogger logger, string userName);
    }
}