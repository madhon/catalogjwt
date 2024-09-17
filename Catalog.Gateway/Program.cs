var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();
builder.AddOpenTelemetry();
builder.RegisterServices();

var app = builder.Build();

app.ConfigureApplication();

await app.RunAsync().ConfigureAwait(false);
