namespace Catalog.Auth.Signup
{
    public static partial class SignUpEndpoint
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
                .Produces<BadRequest>()
                .ProducesValidationProblem()
                .WithOpenApi()
                .RequireRateLimiting(RateLimiterPolicies.RlPoicy);

            return app;
        }

        private static async Task<Results<Ok<SignupResponse>, ProblemHttpResult, BadRequest>> HandleSignup(SignupRequest request,
            IAuthenticationService authenticationService,
            ILoggerFactory loggerFactory,
            CancellationToken ct)
        {
            var logger = loggerFactory.CreateLogger("Catalog.Auth.Signup.SignUpEndpoint");
            
            LogUserSignup(logger, request.Email);
            
            var result = await authenticationService.CreateUser(request.Email, request.Password, request.Fullname, ct).ConfigureAwait(false);

            return !result.IsError ?
                TypedResults.Ok(new SignupResponse(true, "User created successfully")) :
                TypedResults.Problem(title: "Error Creating User",
                    detail: result.Errors[0].Description,
                    statusCode: 400);
        }
        
        [LoggerMessage(0, LogLevel.Information, "User Signed Up {userName}")]
        private static partial void LogUserSignup(ILogger logger, string userName);
    }
}