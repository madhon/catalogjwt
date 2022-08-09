namespace Catalog.Auth.Infrastructure
{
    using Catalog.Auth.Infrastructure.EntityConfigurations;
    using Catalog.Auth.Model;
    using Microsoft.EntityFrameworkCore;

    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }
    }
}
