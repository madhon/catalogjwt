namespace Catalog.API.Infrastructure.Persistence.EntityConfigurations;

using Catalog.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BrandEntityTypeConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("CatalogBrand");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).IsRequired()
            .HasConversion<BrandId.EfCoreValueConverter>();
        builder.Property(b => b.BrandName).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Description).IsRequired().HasMaxLength(int.MaxValue);
    }
}