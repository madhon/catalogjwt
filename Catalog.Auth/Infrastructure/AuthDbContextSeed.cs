namespace Catalog.Auth.Infrastructure
{
    public static class AuthDbContextSeed
    {
        public static async Task SeedAsync(AuthDbContext identityDbContext,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            if (!await roleManager.RoleExistsAsync("read"))
            {
                await roleManager.CreateAsync(new IdentityRole("read"));
            }

            if (!await roleManager.RoleExistsAsync("write"))
            {
                await roleManager.CreateAsync(new IdentityRole("write"));
            }

        }
    }
}
