using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications", "notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(n => n.Type).IsRequired().HasMaxLength(20);
        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Body).IsRequired().HasMaxLength(500);
        builder.Property(n => n.Data).HasMaxLength(500);

        builder.Property(n => n.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Tra cứu feed theo user, mới nhất trước
        builder.HasIndex(n => new { n.UserId, n.CreatedAt });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
