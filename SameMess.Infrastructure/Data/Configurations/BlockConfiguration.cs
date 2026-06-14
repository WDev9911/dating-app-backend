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
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");

        // Má»—i (blocker, blocked) chá»‰ má»™t báº£n ghi
        builder.HasIndex(b => new { b.BlockerId, b.BlockedId })
            .IsUnique();

        // Há»— trá»£ tra cá»©u "ai Ä‘Ã£ block tÃ´i"
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
