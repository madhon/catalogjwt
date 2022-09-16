namespace Catalog.Auth.Infrastructure.EntityConfigurations
{
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class UserEntityTypeConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasConversion<UlidToStringConverter>();
            builder.Property(x => x.Role).IsRequired().HasMaxLength(64);
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.Salt).IsRequired().HasMaxLength(64);
            builder.Property(x => x.Password).IsRequired().HasMaxLength(64);
            builder.Property(x => x.Fullname).HasMaxLength(20).IsRequired();
        }
    }
}