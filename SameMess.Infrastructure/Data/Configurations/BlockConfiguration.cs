using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class BlockConfiguration : IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.ToTable("Blocks", "safety");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Mỗi (blocker, blocked) chỉ một bản ghi
        builder.HasIndex(b => new { b.BlockerId, b.BlockedId })
            .IsUnique();

        // Hỗ trợ tra cứu "ai đã block tôi"
        builder.HasIndex(b => b.BlockedId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(b => b.BlockerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(b => b.BlockedId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
