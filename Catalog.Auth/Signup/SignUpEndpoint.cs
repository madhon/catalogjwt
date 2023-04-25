namespace Catalog.Auth.Signup
{
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class SignUpEndpoint : EndpointBaseAsync.WithRequest<SignupRequest>.WithActionResult<SignupResponse>
    {
        private readonly IAuthenticationService authenticationService;

        public SignUpEndpoint(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [ApiVersion(1.0)]
        [HttpPost("api/v{version:apiVersion}/auth/signup")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Sign Up to API",
            OperationId = "auth.signup",
            Tags = new[] { "Auth" })]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(SignupResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SignupResponse), StatusCodes.Status400BadRequest)]
        public override async Task<ActionResult<SignupResponse>> HandleAsync(SignupRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await authenticationService.CreateUser(request.Email, request.Password, request.Fullname, cancellationToken).ConfigureAwait(false);

            if (!result.IsError && !result.Value.ToString().Any())
            {
                return Ok(new SignupResponse(true, "User created successfully"));
            }
            else
            {
                return BadRequest(new SignupResponse(false, $"Error creating user {result.Value}"));
            }
        }
    }
}
