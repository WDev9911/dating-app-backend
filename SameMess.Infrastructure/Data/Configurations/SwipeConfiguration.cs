using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class SwipeConfiguration : IEntityTypeConfiguration<Swipe>
{
    public void Configure(EntityTypeBuilder<Swipe> builder)
    {
        builder.ToTable("Swipes", "matching");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(s => s.Action)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Mỗi người chỉ swipe một target đúng một lần
        builder.HasIndex(s => new { s.SwiperId, s.TargetUserId })
            .IsUnique();

        // Hỗ trợ tra cứu ngược ("ai đã like tôi" / kiểm tra match)
        builder.HasIndex(s => s.TargetUserId);

        // FK tới Users (Restrict để tránh multiple cascade paths khi 2 FK cùng trỏ về Users)
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.SwiperId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
