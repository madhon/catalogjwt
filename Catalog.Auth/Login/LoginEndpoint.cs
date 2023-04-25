namespace Catalog.Auth.Login
{
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class LoginEndpoint : EndpointBaseAsync
        .WithRequest<LoginRequest>
        .WithActionResult<LoginResponse>
    {
        private readonly IValidator<LoginRequest> loginValidator;
        private readonly IAuthenticationService authenticationService;

        public LoginEndpoint(IAuthenticationService authenticationService, IValidator<LoginRequest> loginValidator)
        {
            this.authenticationService = authenticationService;
            this.loginValidator = loginValidator;
        }

        [ApiVersion(1.0)]
        [HttpPost("api/v{version:apiVersion}/auth/login")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Login to API",
            OperationId = "auth.login",
            Tags = new[] { "Auth" })]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public override async Task<ActionResult<LoginResponse>> HandleAsync(LoginRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var validationResult = await loginValidator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return ValidationProblem();
            }

            var authenticationResult = await authenticationService.Authenticate(request.Email, request.Password).ConfigureAwait(false);

            if (authenticationResult.IsError)
            {
                return Problem(
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

            return Ok(response);
        }
    }
}
