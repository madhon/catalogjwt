namespace Catalog.Auth.Endpoints
{
    public class LoginEndpoint : Endpoint<LoginModel>
    {
        private readonly IAuthenticationService authenticationService;

        public LoginEndpoint(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        public override void Configure()
        {
            Version(1);
            Post("auth/login");
            AllowAnonymous();
        }

        public override async Task HandleAsync(LoginModel req, CancellationToken ct)
        {
            var authenticationResult = await authenticationService.Authenticate(req.Email, req.Password).ConfigureAwait(false);

            if (authenticationResult.IsError)
            {
                await SendAsync(new
                {
                    Succeeded = false,
                    Message = authenticationResult.Errors.First().Description
                }, 403, ct).ConfigureAwait(false);

            }
            else
            {
                var response = new TokenResponse()
                {
                    AccessToken = authenticationResult.Value.Token,
                    ExpiresIn = authenticationResult.Value.ExpiresIn,
                };

                await SendAsync(response);
            }
        }
    }
}
