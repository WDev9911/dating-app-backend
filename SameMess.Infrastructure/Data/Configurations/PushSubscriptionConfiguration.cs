using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class PushSubscriptionConfiguration : IEntityTypeConfiguration<PushSubscription>
{
    public void Configure(EntityTypeBuilder<PushSubscription> builder)
    {
        builder.ToTable("PushSubscriptions", "notifications");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(s => s.Endpoint).IsRequired().HasMaxLength(1000);
        builder.Property(s => s.P256dh).IsRequired().HasMaxLength(300);
        builder.Property(s => s.Auth).IsRequired().HasMaxLength(200);

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Mỗi (user, endpoint) chỉ một đăng ký
        builder.HasIndex(s => new { s.UserId, s.Endpoint }).IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
