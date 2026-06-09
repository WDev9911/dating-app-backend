using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages", "chat");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(m => m.SentAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Hỗ trợ phân trang lịch sử theo thời gian
        builder.HasIndex(m => new { m.ConversationId, m.SentAt });

        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK người gửi tới Users (Restrict để tránh multiple cascade paths)
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
