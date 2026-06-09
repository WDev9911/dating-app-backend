using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class ReputationEventConfiguration : IEntityTypeConfiguration<ReputationEvent>
{
    public void Configure(EntityTypeBuilder<ReputationEvent> builder)
    {
        builder.ToTable("ReputationEvents", "reputation");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(e => e.Severity)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Reason)
            .HasMaxLength(300);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Tra cứu log theo user (tính điểm + xét "đã có sự kiện 1-lần chưa")
        builder.HasIndex(e => new { e.UserId, e.Type });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
