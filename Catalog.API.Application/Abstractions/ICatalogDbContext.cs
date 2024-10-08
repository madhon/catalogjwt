namespace Catalog.API.Application.Abstractions;

public interface ICatalogDbContext
{
    DbSet<Brand> Brands { get; }
    DbSet<Product> Products { get; }

    IEnumerable<Product> GetProducts(int pageSize, int pageIndex);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}