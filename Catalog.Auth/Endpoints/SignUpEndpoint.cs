namespace Catalog.Auth.Endpoints
{
    public class SignUpEndpoint : Endpoint<SignUpModel>
    {
        private readonly IAuthenticationService authenticationService;

        public SignUpEndpoint(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        public override void Configure()
        {
            Version(1);
            Post("auth/signup");
            AllowAnonymous();
        }

        public override async Task HandleAsync(SignUpModel req, CancellationToken ct)
        {
            var result = await authenticationService.CreateUser(req.Email, req.Password, req.Fullname, ct).ConfigureAwait(false);

            if (!result.IsError && !result.Value.ToString().Any())
            {
                await SendAsync(new { Succeeded = true, Message = "User created successfully" }, 200, ct).ConfigureAwait(false);
            }
            else
            {
                await SendAsync(new { Succeeded = false, Message = "Error creating user" }, 200, ct).ConfigureAwait(false);
            }

        }

    }
}
