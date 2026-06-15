using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class SafetyCheckInConfiguration : IEntityTypeConfiguration<SafetyCheckIn>
{
    public void Configure(EntityTypeBuilder<SafetyCheckIn> builder)
    {
        builder.ToTable("SafetyCheckIns", "safety");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.Status).IsRequired().HasMaxLength(20);

        builder.HasIndex(c => new { c.UserId, c.CreatedAt });

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
