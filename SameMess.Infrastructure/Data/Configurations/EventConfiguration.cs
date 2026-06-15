using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events", "events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.Location).HasMaxLength(300);
        builder.Property(e => e.ImageUrl).HasMaxLength(500);
        builder.Property(e => e.Badge).HasMaxLength(100);

        builder.Property(e => e.IsPublished).HasDefaultValue(false);

        builder.HasIndex(e => new { e.IsPublished, e.StartAt });

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");
    }
}
