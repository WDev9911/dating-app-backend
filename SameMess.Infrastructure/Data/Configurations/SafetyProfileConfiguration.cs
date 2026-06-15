using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class SafetyProfileConfiguration : IEntityTypeConfiguration<SafetyProfile>
{
    public void Configure(EntityTypeBuilder<SafetyProfile> builder)
    {
        builder.ToTable("SafetyProfiles", "safety");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.HasIndex(s => s.UserId).IsUnique();

        builder.Property(s => s.PinHash).HasMaxLength(200);
        builder.Property(s => s.AlertMessage).HasMaxLength(500);

        builder.Property(s => s.PinEnabled).HasDefaultValue(false);
        builder.Property(s => s.EmergencyAlertEnabled).HasDefaultValue(false);
        builder.Property(s => s.CheckinEnabled).HasDefaultValue(false);

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
