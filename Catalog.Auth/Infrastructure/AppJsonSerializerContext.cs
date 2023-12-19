namespace Catalog.Auth.Infrastructure;

using System.Text.Json;
using System.Text.Json.Serialization;
using Catalog.Auth.Login;
using Catalog.Auth.Signup;

[JsonSourceGenerationOptions(defaults: JsonSerializerDefaults.Web, GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(LoginResponse))]
[JsonSerializable(typeof(SignupRequest))]
[JsonSerializable(typeof(SignupResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}