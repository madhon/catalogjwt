namespace Catalog.Auth.Login
{
    using Catalog.Auth.Extensions;

    public class LoginEndpoint : EndpointBaseAsync
        .WithRequest<LoginModel>
        .WithActionResult<TokenResponse>
    {
        private readonly IValidator<LoginModel> loginValidator;
        private readonly IAuthenticationService authenticationService;

        public LoginEndpoint(IAuthenticationService authenticationService, IValidator<LoginModel> loginValidator)
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
        [ProducesResponseType(typeof(TokenResponse), (int)HttpStatusCode.OK)]
        public override async Task<ActionResult<TokenResponse>> HandleAsync(LoginModel request, CancellationToken cancellationToken = new CancellationToken())
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

            var response = new TokenResponse
            {
                AccessToken = authenticationResult.Value.Token,
                ExpiresIn = authenticationResult.Value.ExpiresIn,
            };

            return Ok(response);
        }
    }
}
