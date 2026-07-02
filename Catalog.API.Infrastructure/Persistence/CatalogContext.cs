namespace Catalog.API.Infrastructure.Persistence;

using Catalog.API.Application.Abstractions;
using Catalog.API.Domain.Entities;
using Catalog.API.Infrastructure.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

/// <summary>
///  dotnet ef dbcontext optimize --output-dir .\Persistence\CompiledModels --context CatalogContext --namespace Catalog.API.Infrastructure.Persistence.CompiledModels
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
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfiguration(new BrandEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.RegisterAllInVogenEfCoreConverters();
    }

    private static readonly Func<CatalogContext, CancellationToken, Task<long>> GetProductCountCompiled =
        EF.CompileAsyncQuery((CatalogContext catalogDbContext, CancellationToken cancellationToken) =>
            catalogDbContext.Products
                .AsNoTracking()
                .LongCount());

    private static readonly Func<CatalogContext, int, int, IAsyncEnumerable<Product>> GetProductsPageCompiled =
        EF.CompileAsyncQuery(
            (CatalogContext context, int pageSize, int pageIndex) =>
                context.Products
                    .AsNoTracking()
                    .OrderBy(p => p.Name)
                    .Skip(pageSize * pageIndex)
                    .Take(pageSize)
                    .Include(p => p.Brand));

    public async Task<long> GetProductCountAsync(CancellationToken cancellationToken)
    {
        return await GetProductCountCompiled(this, cancellationToken);
    }

    public async Task<IList<Product>> GetAllProducts(int pageSize, int pageIndex, CancellationToken cancellationToken)
    {
        return await GetProductsPageCompiled(this, pageSize, pageIndex)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
