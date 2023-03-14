namespace Catalog.API.Infrastructure.Persistence
{
    using Catalog.API.Application.Common;
    using Catalog.API.Domain.Entities;
    using Catalog.API.Infrastructure.EntityConfigurations;
    using Catalog.API.Infrastructure.Persistence.EntityConfigurations;
    using Microsoft.EntityFrameworkCore;

    public class CatalogContext : DbContext, ICatalogDbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options):base(options)
        {
        }

        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BrandEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
        }
    }
}
