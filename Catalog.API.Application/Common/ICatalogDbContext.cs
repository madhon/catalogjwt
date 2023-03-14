namespace Catalog.API.Application.Common
{
    public interface ICatalogDbContext
    {
        DbSet<Brand> Brands { get; }
        DbSet<Product> Products { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
