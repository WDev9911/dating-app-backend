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
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(m => m.IsActive)
            .HasDefaultValue(true);

        builder.Property(m => m.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Mỗi cặp (đã chuẩn hóa A<B) chỉ có một match — chặn race condition tạo trùng
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
