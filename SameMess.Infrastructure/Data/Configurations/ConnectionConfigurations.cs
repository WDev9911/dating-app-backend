using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SameMess.Domain.Entities;

namespace SameMess.Infrastructure.Data.Configurations;

public class NudgeDismissalConfiguration : IEntityTypeConfiguration<NudgeDismissal>
{
    public void Configure(EntityTypeBuilder<NudgeDismissal> builder)
    {
        builder.ToTable("NudgeDismissals", "chat");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(d => d.NudgeCode).IsRequired().HasMaxLength(50);
        builder.HasIndex(d => new { d.UserId, d.ConversationId, d.NudgeCode }).IsUnique();
        builder.Property(d => d.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
    }
}

public class MeetupProposalConfiguration : IEntityTypeConfiguration<MeetupProposal>
{
    public void Configure(EntityTypeBuilder<MeetupProposal> builder)
    {
        builder.ToTable("MeetupProposals", "chat");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(m => m.VenueId).HasMaxLength(200);
        builder.Property(m => m.Note).HasMaxLength(500);
        builder.Property(m => m.Status).IsRequired().HasMaxLength(20);
        builder.HasIndex(m => m.ConversationId);
        builder.Property(m => m.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
    }
}
