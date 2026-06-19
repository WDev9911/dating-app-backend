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
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(m => m.Type)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.MessageType.Text);

        builder.Property(m => m.SentAt)
            .HasDefaultValueSql("now() at time zone 'utc'");

        // Há»— trá»£ phÃ¢n trang lá»‹ch sá»­ theo thá»i gian
        builder.HasIndex(m => new { m.ConversationId, m.SentAt });

        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK ngÆ°á»i gá»­i tá»›i Users (Restrict Ä‘á»ƒ trÃ¡nh multiple cascade paths)
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
