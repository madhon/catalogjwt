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
            
            await SendAsync(new
            {
                Succeeded = result.Succeeded,
                Message = result.Succeeded ? "User created successfully" : $"User creation failed" 
            }, 200, ct).ConfigureAwait(false);
        }

    }
}
