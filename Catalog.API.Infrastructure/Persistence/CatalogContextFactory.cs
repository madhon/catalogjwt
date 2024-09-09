namespace Catalog.API.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class CatalogContextFactory : IDesignTimeDbContextFactory<CatalogContext>
{
    public CatalogContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CatalogContext>();
        optionsBuilder.UseSqlServer("Server=.;Initial Catalog=CatalogDb;Integrated Security=true;TrustServerCertificate=true;Encrypt=false;");
        return new CatalogContext(optionsBuilder.Options);
    }
}