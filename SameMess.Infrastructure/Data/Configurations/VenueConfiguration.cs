using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("Venues", "chat");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(v => v.Name).IsRequired().HasMaxLength(200);
        builder.Property(v => v.Category).IsRequired().HasMaxLength(30);
        builder.Property(v => v.Address).HasMaxLength(300);
        builder.Property(v => v.District).HasMaxLength(50);
        builder.Property(v => v.City).IsRequired().HasMaxLength(50);
        builder.Property(v => v.ImageUrl).HasMaxLength(500);
        builder.Property(v => v.PriceRange).HasMaxLength(10);
        builder.Property(v => v.Description).HasMaxLength(1000);

        builder.Property(v => v.IsActive).HasDefaultValue(true);
        builder.HasIndex(v => new { v.IsActive, v.Latitude, v.Longitude });

        builder.Property(v => v.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
    }
}
