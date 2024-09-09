namespace Catalog.Auth.Infrastructure;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class AuthDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>, IDataProtectionKeyContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }
        
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

}