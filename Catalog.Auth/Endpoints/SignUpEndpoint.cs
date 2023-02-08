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
            var result = await authenticationService.CreateUser(req.Email, req.Password, req.Fullname, ct).ConfigureAwait(false);
            
            await SendAsync(new
            {
                Succeeded = result.Succeeded,
                Message = result.Succeeded ? "User created successfully" : "User creation failed"
            }, 200, ct).ConfigureAwait(false);
        }

    }
}
