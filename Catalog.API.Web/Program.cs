var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrelHttpsConfiguration();

builder.AddServiceDefaults();
builder.AddDefaultOpenApi();

builder.AddMySerilogLogging();

builder.Services.AddMyApi()
    .AddMyInfrastructureDependencies()
    .AddApplicationServices();

var app = builder.Build();

app.UseDefaultOpenApi();

#pragma warning disable S125
//app.AddDefaultSecurityHeaders();
#pragma warning restore S125

app.UseMyRequestLogging();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseRouting();
app.UseMyInfrastructure();
app.UseMyApi();

await app.RunAsync().ConfigureAwait(false);
