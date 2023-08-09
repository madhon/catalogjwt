var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();
builder.AddOpenTelemetry();
builder.RegisterServices();

var app = builder.Build();

app.ConfigureApplication();

using (var scope = app.Services.CreateScope())
{
    var scopedProvider = scope.ServiceProvider;
    try
    {
        var authContext = scopedProvider.GetRequiredService<AuthDbContext>();
        var userManager = scopedProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await AuthDbContextSeed.SeedAsync(authContext, userManager, roleManager);

    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}

app.Run();