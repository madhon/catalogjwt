namespace Catalog.Auth.Endpoints
{
    using Serilog;

    public class SignUpEndpoint : Endpoint<SignUpModel>
    {
        private readonly IAuthenticationService authenticationService;

        public SignUpEndpoint(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        public override void Configure()
        {
            Post("signup");
            AllowAnonymous();
        }

        public override async Task HandleAsync(SignUpModel req, CancellationToken ct)
        {
            await authenticationService.CreateUser(req.Email, req.Password, req.Fullname, ct).ConfigureAwait(false);

            await SendAsync(new
            {
                Succeeded = true,
                Message = "User Created in successfully"
            }, 200, ct).ConfigureAwait(false);
        }

    }
}
