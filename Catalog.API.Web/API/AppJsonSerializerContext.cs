namespace Catalog.API.Web.API;

using System.Text.Json;
using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(defaults: JsonSerializerDefaults.Web, GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(AddBrandRequest))]
[JsonSerializable(typeof(AddProductRequest))]
[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Brand))]
internal sealed partial class AppJsonSerializerContext : JsonSerializerContext;