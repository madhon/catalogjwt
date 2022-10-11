var builder = WebApplication.CreateBuilder(args);

builder.ConfigureHost();
builder.RegisterServices();

var app = builder.Build();

app.ConfigureApplication();

app.Run();
