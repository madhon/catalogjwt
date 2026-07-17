namespace Catalog.Auth.Infrastructure;

using System.Text.Json;
using System.Text.Json.Serialization;
using Catalog.Auth.Login;
using Catalog.Auth.Refresh;
using Catalog.Auth.Signup;


[JsonSourceGenerationOptions(defaults: JsonSerializerDefaults.Web, GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(LoginResponse))]
[JsonSerializable(typeof(SignupRequest))]
[JsonSerializable(typeof(SignupResponse))]

[JsonSerializable(typeof(RefreshRequest))]
[JsonSerializable(typeof(RefreshResponse))]

[JsonSerializable(typeof(HttpValidationProblemDetails))]
[JsonSerializable(typeof(ValidationProblem))]
[JsonSerializable(typeof(ProblemHttpResult))]
internal sealed partial class AppJsonSerializerContext : JsonSerializerContext;