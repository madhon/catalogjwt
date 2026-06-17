# CatalogJwt Copilot Instructions

## Build, test, and run commands

```powershell
dotnet restore .\Catalog.API.slnx
dotnet build .\Catalog.API.slnx -c Debug --no-restore
dotnet test .\Catalog.API.slnx -c Debug --no-build
dotnet run --project .\Catalog.AppHost\Catalog.AppHost.csproj
```

`dotnet build` is also the analyzer/code-style gate in this repo: `Directory.Build.props` enables `EnforceCodeStyleInBuild`, sets `AnalysisLevel` to `latest-all`, and promotes many analyzer warnings to errors.

There are currently no test projects checked in, so `dotnet test .\Catalog.API.slnx` is the only repo-level test command and there is no single-test command yet.

Useful service-level entry points when you do not need the full Aspire host:

```powershell
dotnet run --project .\Catalog.API.Web\Catalog.API.Web.csproj
dotnet run --project .\Catalog.Auth\Catalog.Auth.csproj
dotnet run --project .\Catalog.Gateway\Catalog.Gateway.csproj
```

## High-level architecture

This solution targets .NET 10 and is organized as an Aspire-based distributed app:

- `Catalog.AppHost` is the local orchestration entry point. It starts `Catalog.Auth` and `Catalog.API.Web`, adds the Docker Compose environment, and defines the active YARP gateway routes in `Program.cs`.
- `Catalog.Gateway` is still a standalone YARP gateway project, but `Catalog.AppHost` currently fronts requests itself and the standalone gateway launch block is commented out in `Catalog.AppHost\Program.cs`.
- `Catalog.API.Web` is the catalog HTTP API. `Program.cs` stays thin and composes three layers: `AddMyApi()` for HTTP/minimal API concerns, `AddMyInfrastructureDependencies()` for auth/persistence, and `AddApplicationServices()` for mediator pipelines and background processing.
- `Catalog.API.Application` contains mediator requests/handlers plus pipeline behaviors for logging, slow-request warnings, and FusionCache caching. Product writes also flow through a `Channel<Product>` processed by `AddProductChannelProcessor`.
- `Catalog.API.Infrastructure` owns catalog persistence and JWT validation. It uses pooled EF Core contexts, a compiled model, a slow-query interceptor, SQL Server, and `ICatalogDbContext` as the application-facing abstraction.
- `Catalog.API.Domain` holds domain entities and strong IDs such as `BrandId` and `ProductId`.
- `Catalog.Auth` is a separate auth service using ASP.NET Core Identity, its own `AuthDbContext`, persisted data-protection keys, JWT issuance, and startup seeding for the `read` and `write` roles.
- `Catalog.ServiceDefaults` provides shared OpenTelemetry, health checks, service discovery, Prometheus, Scalar/OpenAPI setup, and default security-header helpers.

## Key conventions

- Keep `Program.cs` files thin. Most changes belong in extension/composition files such as `RegisterServices`, `ConfigureApplication`, `AddMyApi`, `UseMyApi`, or the infrastructure startup classes.
- Minimal APIs are the default style. Routes are grouped under versioned prefixes (`api/v1/...`) and use typed results plus explicit metadata (`Produces`, `ProducesProblem`, auth requirements, rate limiting).
- The repo relies heavily on source generators and generated helpers:
  - `ServiceScan` generates validator registration from `[GenerateServiceRegistrations]`.
  - `Mapperly` generates DTO/entity mappers from `[Mapper]`.
  - `Vogen` generates strong IDs with EF Core and `System.Text.Json` converters.
  - Auth also uses `AutoRegisterInject` for service discovery.
  - Both web services insert generated `JsonSerializerContext` instances at the front of the resolver chain.
- Catalog request handling uses the `Mediator` library, not MediatR. Cross-cutting behavior lives in pipeline behaviors. If a query should be cached, implement `IFusionCacheRequest<TResponse>` and provide `CacheKey`/`Tags`.
- Preserve the EF Core performance setup when touching persistence: both auth and catalog use `AddDbContextPool(...)` and `UseModel(...)` compiled models; catalog also uses a compiled query for paged product reads.
- JWT configuration is shared conceptually across services but not under the same config section name: catalog API validation binds `AuthenticationSettings`, while auth and gateway bind `jwt`.
- `x-correlation-id` header propagation and forwarded headers are enabled in the HTTP services; keep that intact when adding outbound HTTP calls or proxy-facing middleware.
- OpenAPI/Scalar is configuration-driven through `Catalog.ServiceDefaults`. In the auth service it is only enabled in development; in the catalog API it is enabled when the `OpenApi` section exists.
