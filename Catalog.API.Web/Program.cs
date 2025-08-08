var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrelHttpsConfiguration();

builder.AddServiceDefaults();
builder.AddDefaultOpenApi();

builder.AddMySerilogLogging();

var startup = new Startup(builder.Configuration, builder.Environment);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

app.UseDefaultOpenApi();

#pragma warning disable S125
//app.AddDefaultSecurityHeaders();
#pragma warning restore S125

startup.Configure(app);

await app.RunAsync().ConfigureAwait(false);
