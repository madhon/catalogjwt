namespace Catalog.API.Web.API.Validators
{
    using Catalog.API.Web.API.Endpoints.Requests;
    using FluentValidation;

    public class AddBrandValidator : Validator<AddBrandRequest>
    {
        public AddBrandValidator()
        {
            RuleFor(x => x.BrandName)
                .NotEmpty()
                .WithMessage("Brand Name is Required")
                .MinimumLength(2)
                .WithMessage("Brand Name is too short");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Brand Description is Required")
                .MinimumLength(2)
                .WithMessage("Brand Description is too short");
        }
    }
}
