namespace Catalog.API.Web.Extensions;

using FluentValidation.Results;
using ServiceScan.SourceGenerator;

internal static partial class FluentValidationExtensions
{
    [GenerateServiceRegistrations(AssignableTo = typeof(IValidator<>), Lifetime = ServiceLifetime.Singleton)]
    public static partial IServiceCollection AddValidators(this IServiceCollection services);

    public static IDictionary<string, string[]> ToDictionary(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(x => x.PropertyName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
, StringComparer.OrdinalIgnoreCase);
    }
}