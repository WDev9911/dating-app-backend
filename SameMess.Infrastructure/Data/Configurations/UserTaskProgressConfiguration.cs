using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class UserTaskProgressConfiguration : IEntityTypeConfiguration<UserTaskProgress>
{
    public void Configure(EntityTypeBuilder<UserTaskProgress> builder)
    {
        builder.ToTable("UserTaskProgress", "gamification");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(p => p.TaskCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.PeriodKey)
            .IsRequired()
            .HasMaxLength(20);

        // Mỗi (user, nhiệm vụ, kỳ) chỉ một dòng tiến độ
        builder.HasIndex(p => new { p.UserId, p.TaskCode, p.PeriodKey })
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
