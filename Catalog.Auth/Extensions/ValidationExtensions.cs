namespace Catalog.Auth.Extensions;

using System.Runtime.InteropServices;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

internal static class ValidationExtensions
{
    internal static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
    {
        foreach (ref var error in CollectionsMarshal.AsSpan(result.Errors))
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }

    internal static IDictionary<string, string[]> ToDictionary(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(x => x.PropertyName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
                ,StringComparer.OrdinalIgnoreCase
            );
    }
}