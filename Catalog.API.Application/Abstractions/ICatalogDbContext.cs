namespace Catalog.API.Application.Abstractions;

public interface ICatalogDbContext
{
    DbSet<Brand> Brands { get; }
    DbSet<Product> Products { get; }

    Task<long> GetProductCountAsync(CancellationToken cancellationToken);
    Task<IList<Product>> GetAllProducts(int pageSize, int pageIndex, CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
