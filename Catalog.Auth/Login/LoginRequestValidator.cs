namespace Catalog.Auth.Login;

#pragma warning disable CA1515
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
#pragma warning restore CA1515
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}