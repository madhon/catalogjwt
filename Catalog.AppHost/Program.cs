var builder = DistributedApplication.CreateBuilder(args);

var authDb = builder.AddSqlServerConnection("AuthDb", "Server=.;Database=AuthDb;Trusted_Connection=True;");

var authService = builder.AddProject<Projects.Catalog_Auth>("auth")
    .WithReference(authDb);

var catalogDb = builder.AddSqlServerConnection("CatalogDb", "Server=.;Database=CatalogDb;Trusted_Connection=True;");

var catalogService = builder.AddProject<Projects.Catalog_API_Web>("catalog")
    .WithReference(catalogDb)
    .WithReference(authService);

builder.AddProject<Projects.Catalog_Gateway>("gateway")
    .WithReference(authService)
    .WithReference(catalogService);

builder.Build().Run();
