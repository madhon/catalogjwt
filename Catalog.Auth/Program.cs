var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrelHttpsConfiguration();

builder.AddServiceDefaults();

builder.AddSerilog();

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
        await AuthDbContextSeed.SeedAsync(authContext, userManager, roleManager).ConfigureAwait(false);

    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}

app.Run();