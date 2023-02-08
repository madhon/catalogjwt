namespace Catalog.Auth.Infrastructure
{
    
    using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class AuthContext : IdentityDbContext<ApplicationUser, IdentityRole, string>, IDataProtectionKeyContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
        }
        
        public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    }
}
