namespace Catalog.Auth.Signup
{
    public class SignUpRequestValidator : AbstractValidator<SignupRequest>
    {
        public SignUpRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Fullname).NotEmpty().MaximumLength(20);
        }
    }
}
