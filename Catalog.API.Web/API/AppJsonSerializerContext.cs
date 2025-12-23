namespace Catalog.API.Web.API;

using Catalog.API.Application.Features.Products;
using Catalog.API.Web.API.ViewModel;
using Microsoft.AspNetCore.Mvc;

[JsonSourceGenerationOptions(defaults: JsonSerializerDefaults.Web, GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(AddBrandRequest))]
[JsonSerializable(typeof(AddProductRequest))]
[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Brand))]
[JsonSerializable(typeof(BrandId))]
[JsonSerializable(typeof(ProductId))]
[JsonSerializable(typeof(PaginatedItemsViewModel<Product>))]
[JsonSerializable(typeof(PaginatedItemsViewModel<Brand>))]

[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(HttpValidationProblemDetails))]
[JsonSerializable(typeof(ValidationProblem))]
[JsonSerializable(typeof(ProblemHttpResult))]

[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(bool?))]
[JsonSerializable(typeof(double?))]
[JsonSerializable(typeof(int?))]
[JsonSerializable(typeof(string))]

internal sealed partial class AppJsonSerializerContext : JsonSerializerContext;