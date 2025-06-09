using Catalog.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddForwardedHeaders();

var authService = builder.AddProject<Projects.Catalog_Auth>("auth");

var catalogService = builder.AddProject<Projects.Catalog_API_Web>("catalog")
    .WithReference(authService)
    .WaitFor(authService);

builder.AddProject<Projects.Catalog_Gateway>("gateway")
    .WithReference(authService)
    .WaitFor(authService)
    .WithReference(catalogService)
    .WaitFor(catalogService);

builder.AddDockerComposeEnvironment("catalog-env");

await builder.Build().RunAsync().ConfigureAwait(false);
