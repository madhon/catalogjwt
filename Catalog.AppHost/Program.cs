using Aspire.Hosting.Yarp;
using Aspire.Hosting.Yarp.Transforms;
using Catalog.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddForwardedHeaders();

var authService = builder.AddProject<Projects.Catalog_Auth>("auth");

var catalogService = builder.AddProject<Projects.Catalog_API_Web>("catalog")
    .WithReference(authService)
    .WaitFor(authService)
    .PublishAsDockerComposeService((resource, service) => { });

builder.AddYarp("gateway-yarp")
    .WithReference(authService)
    .WithReference(catalogService)
    .WaitFor(authService)
    .WaitFor(catalogService)
    .WithHostPort(7060)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/healthz", 404)
    .WithConfiguration(yarp =>
    {
        // Auth Routes
        yarp.AddRoute("/gateway/auth", authService)
            .WithMatchMethods("POST")
            .WithTransformPathSet("/api/v1/auth/login");

        yarp.AddRoute("/gateway/auth/signup", authService)
            .WithMatchMethods("POST")
            .WithTransformPathSet("/api/v1/auth/signup");

        // Catalog Routes
        yarp.AddRoute("/gateway/products/{pageSize}/{pageIndex}", catalogService)
            .WithMatchMethods("GET")
            .WithTransformPathSet("/api/v1/catalog/products/{pageSize}/{pageIndex}");

        yarp.AddRoute("/gateway/brands/add", catalogService)
            .WithMatchMethods("POST")
            .WithTransformPathSet("/api/v1/catalog/addBrand");

        yarp.AddRoute("/gateway/products/add", catalogService)
            .WithMatchMethods("POST")
            .WithTransformPathSet("/api/v1/catalog/addProduct");
    });

// builder.AddProject<Projects.Catalog_Gateway>("gateway")
//     .WithReference(authService)
//     .WaitFor(authService)
//     .WithReference(catalogService)
//     .WaitFor(catalogService)
//     .PublishAsDockerComposeService((resource, service) => { });

builder.AddDockerComposeEnvironment("catalog-env");

await builder.Build().RunAsync().ConfigureAwait(false);
