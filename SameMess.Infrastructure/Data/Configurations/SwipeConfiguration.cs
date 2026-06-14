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
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.Action)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");

        // Má»—i ngÆ°á»i chá»‰ swipe má»™t target Ä‘Ãºng má»™t láº§n
        builder.HasIndex(s => new { s.SwiperId, s.TargetUserId })
            .IsUnique();

        // Há»— trá»£ tra cá»©u ngÆ°á»£c ("ai Ä‘Ã£ like tÃ´i" / kiá»ƒm tra match)
        builder.HasIndex(s => s.TargetUserId);

        // FK tá»›i Users (Restrict Ä‘á»ƒ trÃ¡nh multiple cascade paths khi 2 FK cÃ¹ng trá» vá» Users)
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
