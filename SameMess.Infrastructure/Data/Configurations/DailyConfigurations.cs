using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class UserXpConfiguration : IEntityTypeConfiguration<UserXp>
{
    public void Configure(EntityTypeBuilder<UserXp> builder)
    {
        builder.ToTable("UserXp", "gamification");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.Property(x => x.UpdatedAt).HasDefaultValueSql("now() at time zone 'utc'");
    }
}

public class DailyQuestCompletionConfiguration : IEntityTypeConfiguration<DailyQuestCompletion>
{
    public void Configure(EntityTypeBuilder<DailyQuestCompletion> builder)
    {
        builder.ToTable("DailyQuestCompletions", "gamification");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(c => c.QuestCode).IsRequired().HasMaxLength(50);
        builder.Property(c => c.PeriodKey).IsRequired().HasMaxLength(8);
        builder.HasIndex(c => new { c.UserId, c.QuestCode, c.PeriodKey }).IsUnique();
        builder.Property(c => c.CompletedAt).HasDefaultValueSql("now() at time zone 'utc'");
    }
}
