namespace Catalog.API.Infrastructure.Persistence;

using Catalog.API.Application.Abstractions;
using Catalog.API.Domain.Entities;
using Catalog.API.Infrastructure.EntityConfigurations;
using Catalog.API.Infrastructure.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

/// <summary>
///  dotnet ef dbcontext optimize --output-dir .\Persistence\CompiledModels --data-context CatalogContext --namespace Catalog.API.Infrastructure.Persistence.CompiledModels --force
/// </summary>
public class CatalogContext : DbContext, ICatalogDbContext
{
    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
    {
    }

    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BrandEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
    }
        
    private static readonly Func<CatalogContext, int, int, IEnumerable<Product>> GetProductsInternal =
        EF.CompileQuery(
            (CatalogContext context, int pageSize, int pageIndex) =>
                context.Products
                    .AsNoTracking()
                    .Where(x =>
                        context.Products
                            .OrderBy(c => c.Name)
                            .Select(y => y.Id)
                            .Skip(pageSize * pageIndex)
                            .Take(pageSize)
                            .Contains(x.Id)).Include(x=>x.Brand)
        );

    public IEnumerable<Product> GetProducts(int pageSize, int pageIndex)
    {
        return GetProductsInternal(this, pageSize, pageIndex);
    }

}