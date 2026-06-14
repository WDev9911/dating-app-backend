using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class MatchPlantConfiguration : IEntityTypeConfiguration<MatchPlant>
{
    public void Configure(EntityTypeBuilder<MatchPlant> builder)
    {
        builder.ToTable("MatchPlants", "gamification");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(p => p.Level)
            .HasDefaultValue(1);

        builder.Property(p => p.FreezeUsedWeekKey)
            .HasMaxLength(12);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");

        // 1-1 vá»›i Match
        builder.HasIndex(p => p.MatchId)
            .IsUnique();

        builder.HasOne<Match>()
            .WithMany()
            .HasForeignKey(p => p.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
