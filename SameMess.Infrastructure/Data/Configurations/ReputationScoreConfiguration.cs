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
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.Tier)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.UpdatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");

        // Má»—i user má»™t dÃ²ng Ä‘iá»ƒm
        builder.HasIndex(s => s.UserId)
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
