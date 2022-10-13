namespace Catalog.Auth.ViewModel
{
    public class SignUpModelValidator : Validator<SignUpModel>
    {
        public SignUpModelValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Fullname).NotEmpty().MaximumLength(20);
        }
    }
}
