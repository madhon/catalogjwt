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
            Post("login");
            AllowAnonymous();
        }

        public override async Task HandleAsync(LoginModel req, CancellationToken ct)
        {
            var login = await authenticationService.Authenticate(req.Email, req.Password).ConfigureAwait(false);
            if (login == null)
            {
                await SendAsync(new
                {
                    Succeeded = false,
                    Message = "User not found"
                }, 403, ct).ConfigureAwait(false);
            }
            else
            {
                var response = new TokenResponse
                {
                    AccessToken = login.Token,
                    ExpiresIn = login.ExpiresIn
                };

                await SendAsync(response, 200, ct).ConfigureAwait(false);
            }
        }
    }
}
