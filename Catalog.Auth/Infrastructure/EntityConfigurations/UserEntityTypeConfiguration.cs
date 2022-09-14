﻿namespace Catalog.Auth.Infrastructure.EntityConfigurations
{
    using Catalog.Auth.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class UserEntityTypeConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasConversion<UlidToStringConverter>();
            builder.Property(x => x.Role).IsRequired();
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.Fullname).HasMaxLength(20).IsRequired();
        }
    }
}