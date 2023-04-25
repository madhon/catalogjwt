var builder = WebApplication.CreateBuilder(args);

builder.Host.AddMySerilogLogging();

var startup = new Startup(builder.Configuration, builder.Environment);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app);

app.Run();
