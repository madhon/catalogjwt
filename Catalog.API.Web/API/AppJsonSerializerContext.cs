namespace Catalog.API.Web.API;

using Catalog.API.Web.API.ViewModel;

[JsonSourceGenerationOptions(defaults: JsonSerializerDefaults.Web, GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(AddBrandRequest))]
[JsonSerializable(typeof(AddProductRequest))]
[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Brand))]
[JsonSerializable(typeof(PaginatedItemsViewModel<Product>))]
[JsonSerializable(typeof(PaginatedItemsViewModel<Brand>))]

[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(int))]

internal sealed partial class AppJsonSerializerContext : JsonSerializerContext;