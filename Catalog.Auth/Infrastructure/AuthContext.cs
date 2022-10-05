namespace Catalog.Auth.Infrastructure
{
    using Catalog.Auth.Infrastructure.EntityConfigurations;
    using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

    public class AuthContext : DbContext, IDataProtectionKeyContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }
    }
}
