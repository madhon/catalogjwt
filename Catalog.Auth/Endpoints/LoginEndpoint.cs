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
            var login = authenticationService.Authenticate(req.Email, req.Password);
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
                await SendAsync(new
                {
                    Result = login,
                    Succeeded = true,
                    Message = "User logged in successfully"
                }, 200, ct).ConfigureAwait(false);
            }
        }
    }
}
