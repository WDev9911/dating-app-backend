using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations", "chat");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Mỗi match chỉ có một conversation
        builder.HasIndex(c => c.MatchId)
            .IsUnique();

        builder.HasOne(c => c.Match)
            .WithMany()
            .HasForeignKey(c => c.MatchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
