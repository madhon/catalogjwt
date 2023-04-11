namespace Catalog.Auth.Endpoints
{
    public class SignUpEndpoint : EndpointBaseAsync.WithRequest<SignUpModel>.WithActionResult
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
        public override async Task<ActionResult> HandleAsync(SignUpModel request, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await authenticationService.CreateUser(request.Email, request.Password, request.Fullname, cancellationToken).ConfigureAwait(false);

            if (!result.IsError && !result.Value.ToString().Any())
            {
                return Ok(new { Succeeded = true, Message = "User created successfully" });
            }
            else
            {
                return BadRequest(new { Succeeded = false, Message = $"Error creating user {result.Value}" });
            }
        }
    }
}
