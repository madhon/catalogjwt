namespace Catalog.API.Infrastructure
{
    using Catalog.API.Infrastructure.EntityConfigurations;
    using Catalog.API.Model;
    using Microsoft.EntityFrameworkCore;

    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options):base(options)
        {
        }

        public DbSet<Brand> Brand => Set<Brand>();
        public DbSet<Product> Product => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BrandEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
        }
    }
}
