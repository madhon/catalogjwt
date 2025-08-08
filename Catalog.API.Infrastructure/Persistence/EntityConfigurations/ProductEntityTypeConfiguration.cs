namespace Catalog.API.Infrastructure.Persistence.EntityConfigurations;

using Catalog.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Product");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .IsRequired()
            .HasConversion<ProductId.EfCoreValueConverter>();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(int.MaxValue);
        builder.Property(p => p.Price).IsRequired().HasPrecision(18,2);

        builder.Property(p => p.PictureUri).IsRequired(false).HasMaxLength(int.MaxValue);

        builder.Property(b => b.BrandId).IsRequired()
            .HasConversion<BrandId.EfCoreValueConverter>();
        builder.HasOne(p => p.Brand).WithMany().HasForeignKey(p => p.BrandId);
    }
}