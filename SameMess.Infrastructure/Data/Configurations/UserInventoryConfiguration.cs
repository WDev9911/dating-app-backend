using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class UserInventoryConfiguration : IEntityTypeConfiguration<UserInventory>
{
    public void Configure(EntityTypeBuilder<UserInventory> builder)
    {
        builder.ToTable("UserInventories", "gamification");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(i => i.MaterialType)
            .IsRequired()
            .HasMaxLength(20);

        // Má»—i (user, loáº¡i nguyÃªn liá»‡u) chá»‰ má»™t dÃ²ng
        builder.HasIndex(i => new { i.UserId, i.MaterialType })
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
