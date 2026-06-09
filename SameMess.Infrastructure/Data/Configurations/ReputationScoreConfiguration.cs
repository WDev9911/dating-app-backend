using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class ReputationScoreConfiguration : IEntityTypeConfiguration<ReputationScore>
{
    public void Configure(EntityTypeBuilder<ReputationScore> builder)
    {
        builder.ToTable("ReputationScores", "reputation");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(s => s.Tier)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Mỗi user một dòng điểm
        builder.HasIndex(s => s.UserId)
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
