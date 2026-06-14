using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("Matches", "matching");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(m => m.IsActive)
            .HasDefaultValue(true);

        builder.Property(m => m.CreatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");

        // Má»—i cáº·p (Ä‘Ã£ chuáº©n hÃ³a A<B) chá»‰ cÃ³ má»™t match â€” cháº·n race condition táº¡o trÃ¹ng
        builder.HasIndex(m => new { m.UserAId, m.UserBId })
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.UserAId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.UserBId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
