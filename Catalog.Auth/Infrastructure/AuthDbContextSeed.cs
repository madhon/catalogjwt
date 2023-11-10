namespace Catalog.Auth.Infrastructure
{
    public static class AuthDbContextSeed
    {
        public static async Task SeedAsync(AuthDbContext identityDbContext,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            if (!await roleManager.RoleExistsAsync("read").ConfigureAwait(false))
            {
                await roleManager.CreateAsync(new IdentityRole("read")).ConfigureAwait(false);
            }

            if (!await roleManager.RoleExistsAsync("write").ConfigureAwait(false))
            {
                await roleManager.CreateAsync(new IdentityRole("write")).ConfigureAwait(false);
            }

        }
    }
}
