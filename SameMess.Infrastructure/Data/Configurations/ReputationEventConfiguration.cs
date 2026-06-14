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
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(e => e.Severity)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Reason)
            .HasMaxLength(300);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");

        // Tra cá»©u log theo user (tÃ­nh Ä‘iá»ƒm + xÃ©t "Ä‘Ã£ cÃ³ sá»± kiá»‡n 1-láº§n chÆ°a")
        builder.HasIndex(e => new { e.UserId, e.Type });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
