using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class VenueComboConfiguration : IEntityTypeConfiguration<VenueCombo>
{
    public void Configure(EntityTypeBuilder<VenueCombo> builder)
    {
        builder.ToTable("VenueCombos", "chat");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.Title).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Description).HasMaxLength(500);
        builder.Property(c => c.IsActive).HasDefaultValue(true);
        builder.Property(c => c.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");

        builder.HasOne(c => c.Venue)
            .WithMany()
            .HasForeignKey(c => c.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => new { c.VenueId, c.IsActive });
    }
}

public class DatePassOrderConfiguration : IEntityTypeConfiguration<DatePassOrder>
{
    public void Configure(EntityTypeBuilder<DatePassOrder> builder)
    {
        builder.ToTable("DatePassOrders", "billing");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(o => o.VenueName).IsRequired().HasMaxLength(200);
        builder.Property(o => o.ComboTitle).IsRequired().HasMaxLength(150);
        builder.Property(o => o.VoucherCode).IsRequired().HasMaxLength(40);
        builder.Property(o => o.Email).HasMaxLength(256);
        builder.Property(o => o.Status).IsRequired().HasMaxLength(20);
        builder.Property(o => o.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");

        builder.HasIndex(o => o.VoucherCode).IsUnique();
        builder.HasIndex(o => o.MatchId);
    }
}
