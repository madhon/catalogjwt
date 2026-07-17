namespace Catalog.Auth.Infrastructure;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id).ValueGeneratedOnAdd();
        builder.Property(rt => rt.Token).IsRequired().HasMaxLength(128);
        builder.Property(rt => rt.UserId).IsRequired().HasMaxLength(128);
        builder.Property(rt => rt.Created).IsRequired();
        builder.Property(rt => rt.Expires).IsRequired();
        builder.Property(rt => rt.Revoked);
        builder.Property(rt => rt.ReplacedByToken).HasMaxLength(128);
    }
}