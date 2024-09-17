var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrelHttpsConfiguration();

builder.AddServiceDefaults();
builder.AddDefaultOpenApi();

builder.Host.AddMySerilogLogging();

var startup = new Startup(builder.Configuration, builder.Environment);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

app.UseDefaultOpenApi();

startup.Configure(app);

await app.RunAsync().ConfigureAwait(false);
