namespace Catalog.Auth.Infrastructure;

using System.Text.Json;
using System.Text.Json.Serialization;
using Catalog.Auth.Login;
using Catalog.Auth.Signup;
using Microsoft.AspNetCore.Mvc;

[JsonSourceGenerationOptions(defaults: JsonSerializerDefaults.Web, GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(LoginResponse))]
[JsonSerializable(typeof(SignupRequest))]
[JsonSerializable(typeof(SignupResponse))]

[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(HttpValidationProblemDetails))]
[JsonSerializable(typeof(ValidationProblem))]
[JsonSerializable(typeof(ProblemHttpResult))]
internal sealed partial class AppJsonSerializerContext : JsonSerializerContext;